<#
.SYNOPSIS
    Smoke test for a running i18nApp site.
.EXAMPLE
    .\scripts\smoke-test.ps1 -BaseUrl https://localhost:44322
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$BaseUrl
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Net.Http

# Follow nothing and keep no cookies, so that each check sees the raw response.
$handler = New-Object System.Net.Http.HttpClientHandler
$handler.AllowAutoRedirect = $false
$handler.UseCookies = $false
$client = New-Object System.Net.Http.HttpClient($handler)
$client.BaseAddress = New-Object System.Uri($BaseUrl)

$script:failures = 0

function Get-Page([string]$Path, [hashtable]$Headers = @{}) {
    $request = New-Object System.Net.Http.HttpRequestMessage([System.Net.Http.HttpMethod]::Get, $Path)
    foreach ($name in $Headers.Keys) {
        [void]$request.Headers.TryAddWithoutValidation($name, $Headers[$name])
    }
    $response = $client.SendAsync($request).GetAwaiter().GetResult()
    $setCookie = $null
    $values = $null
    if ($response.Headers.TryGetValues('Set-Cookie', [ref]$values)) {
        $setCookie = ($values | Where-Object { $_ -like 'culture=*' }) -join '; '
    }
    [pscustomobject]@{
        Headers   = $response.Headers
        Status    = [int]$response.StatusCode
        Location  = if ($response.Headers.Location) { $response.Headers.Location.OriginalString } else { $null }
        SetCookie = $setCookie
        Body      = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    }
}

function Assert-That([bool]$Condition, [string]$Message) {
    if ($Condition) {
        Write-Host "PASS $Message"
    }
    else {
        Write-Host "FAIL $Message"
        $script:failures++
    }
}

$page = Get-Page '/' @{ 'Accept-Language' = 'en' }
Assert-That ($page.Status -eq 200 -and $page.Body.Contains('I18n, internationalization!!')) '/ is English for Accept-Language: en'

$page = Get-Page '/' @{ 'Accept-Language' = 'ja' }
Assert-That ($page.Status -eq 200 -and $page.Body.Contains('国際化対応！！')) '/ is Japanese for Accept-Language: ja'

$page = Get-Page '/Home/About?page=1'
Assert-That ($page.Status -eq 302 -and $page.Location -eq '/ja/Home/About?page=1') "/Home/About redirects to /ja/Home/About?page=1 (got $($page.Status) $($page.Location))"

$page = Get-Page '/Home/About' @{ 'Cookie' = 'culture=en' }
Assert-That ($page.Location -eq '/en/Home/About') "/Home/About with culture=en cookie redirects to /en/Home/About (got $($page.Location))"

$page = Get-Page '/Home/About' @{ 'Cookie' = 'culture=evil.example' }
Assert-That ($page.Location -eq '/ja/Home/About') "/Home/About ignores an unsupported culture cookie (got $($page.Location))"

$page = Get-Page '/ja/Home/About'
Assert-That ($page.Status -eq 200 -and $page.Body.Contains('アバウトページ！')) '/ja/Home/About is Japanese'
Assert-That ($page.SetCookie -match '^culture=ja;' -and $page.SetCookie -match 'HttpOnly' -and $page.SetCookie -match 'SameSite=Lax') "/ja/Home/About sets an HttpOnly, SameSite=Lax culture cookie (got $($page.SetCookie))"

$page = Get-Page '/en/Home/Contact'
Assert-That ($page.Status -eq 200 -and $page.Body.Contains('Contact page!')) '/en/Home/Contact is English'

foreach ($name in 'X-AspNet-Version', 'X-AspNetMvc-Version', 'X-Powered-By') {
    Assert-That (-not $page.Headers.Contains($name)) "$name header is not sent"
}
$values = $null
Assert-That ($page.Headers.TryGetValues('X-Content-Type-Options', [ref]$values) -and ($values -join ',') -eq 'nosniff') 'X-Content-Type-Options is nosniff'
$values = $null
Assert-That ($page.Headers.TryGetValues('X-Frame-Options', [ref]$values) -and ($values -join ',') -eq 'SAMEORIGIN') 'X-Frame-Options is SAMEORIGIN'

if ($script:failures -gt 0) {
    Write-Host "$($script:failures) check(s) failed."
    exit 1
}
Write-Host 'All checks passed.'
