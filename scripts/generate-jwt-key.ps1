<#
.SYNOPSIS
    Generates a cryptographically random key suitable for signing JWTs (Jwt:Key), and nothing else -
    it only prints the value. Use setup-local-dev.ps1 to generate one and store it in user-secrets
    in a single step, or pipe/paste this output into your own `dotnet user-secrets set` /
    Jwt__Key environment variable for another environment.

.EXAMPLE
    ./scripts/generate-jwt-key.ps1
#>

# 64 random bytes (512 bits) base64-encoded - comfortably above the 256-bit minimum HS256 needs,
# so there's no need to reason about exact bit-length when rotating this later.
# RNGCryptoServiceProvider (not the newer RandomNumberGenerator.Fill) so this also runs under
# Windows PowerShell 5.1, which is on .NET Framework rather than modern .NET.
$bytes = New-Object byte[] 64
$rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::new()
try {
    $rng.GetBytes($bytes)
} finally {
    $rng.Dispose()
}
[Convert]::ToBase64String($bytes)
