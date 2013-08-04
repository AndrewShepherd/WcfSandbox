set-location C:\Users\Andrew\Documents\GitHub\WcfSandbox

#Set environment variables for Visual Studio Command Prompt
pushd 'c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC'
cmd /c "vcvarsall.bat&set" |
foreach {
  if ($_ -match "=") {
    $v = $_.split("="); set-item -force -path "ENV:\$($v[0])"  -value "$($v[1])"
  }
}
popd
write-host "`nVisual Studio 2010 Command Prompt variables set." -ForegroundColor Yellow


# This sets up the certificate
# Would be good if I can specify "No Password"
makecert -n "CN=RootCATest" -r -sv RootCATest.pvk RootCATest.cer
