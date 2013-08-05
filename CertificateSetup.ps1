 # These values will have to change depending upon the location
 
 $workingLocation = 'C:\Users\ashepherd\Documents\GitHub\WcfSandbox';
 
 $visualStudioLocation = 'C:\Program Files (x86)\Microsoft Visual Studio 11.0';

 
 set-location $workingLocation

#Set environment variables for Visual Studio Command Prompt
$vsCommandPrompt = (join-path -Path  $visualStudioLocation -ChildPath "VC")


pushd $vsCommandPrompt
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

# Create a "certificate revocation list"
makecert -crl -n "CN=RootCATest" -r -sv RootCATest.pvk RootCATest.crl

# This creates the key. Make sure you are running powershell with admin right
makecert -sk MyKeyName -iv RootCATest.pvk -n "CN=tempCert" -ic RootCATest.cer -sr localmachine -ss my -sky exchange -pe 