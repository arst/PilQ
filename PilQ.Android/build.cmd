nuget install FAKE -OutputDirectory packages -ExcludeVersion
call "packages/FAKE/tools/Fake.exe" build.fsx 