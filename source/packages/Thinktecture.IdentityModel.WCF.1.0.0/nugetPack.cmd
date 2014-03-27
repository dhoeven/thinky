xcopy ..\..\build\thinktecture.identitymodel.wcf*.* lib\net45 /y
NuGet.exe pack Thinktecture.IdentityModel.WCF.nuspec -OutputDirectory ..\