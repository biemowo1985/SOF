@echo  off

set MsiVersion=1.0.2.0
set MsBuildPath=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
set ProjectRootDir=D:\PracticeProjects\Github\SOF\WixSolution
set	SolutionDir=%ProjectRootDir%\WixSolution.sln

call %MsBuildPath% %SolutionDir% /property:Configuration=Debug /t:clean /p:VisualStudioVersion=14.0
call %MsBuildPath% %SolutionDir% /property:Configuration=Debug /t:build /p:VisualStudioVersion=14.0 /p:MsiVersion=%MsiVersion%
@pause
