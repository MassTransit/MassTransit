param($installPath, $toolsPath, $package, $project)

# $project = Get-Project

$libzmq = $project.ProjectItems.Item("libzmq.dll")

# set Build Action to None
$libzmq.Properties.Item("BuildAction").Value = 0

# set Copy to Output Directy to Copy if newer
$libzmq.Properties.Item("CopyToOutputDirectory").Value = 2
