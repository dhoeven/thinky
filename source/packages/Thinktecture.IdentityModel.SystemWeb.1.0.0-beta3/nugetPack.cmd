xcopy ..\..\build\thinktecture.identitymodel.systemweb*.* lib\net45 /y
NuGet.exe pack Thinktecture.IdentityModel.SystemWeb.nuspec -OutputDirectory ..\