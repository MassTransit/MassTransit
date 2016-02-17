param($installPath, $toolsPath, $package, $project)

if(!$toolsPath){
    $project = Get-Project
}

function Search-Specification($element, $typeName) {
    if($element.IsCodeType -and ($element.Kind -eq [EnvDTE.vsCMElement]::vsCMElementClass)) {
        foreach($interface in $element.ImplementedInterfaces) {
            if($interface.FullName -eq $typeName) {
                return $element
            }

            $result = Search-Specification -element $interface -typeName $typeName
            if($result -ne $null) {
                return $result
            }
        }
    }
    elseif($element.Kind -eq [EnvDTE.vsCMElement]::vsCMElementNamespace) {
        foreach($member in $element.Members) {
            $result = Search-Specification -element $member -typeName $typeName
            if($result -ne $null) {
                return $result
            }
        }
    }

    $null
}

function Find-SpecificationInItem($item, $typeName) {
    foreach($element in $item.FileCodeModel.CodeElements) {
        $result = Search-Specification -element $element -typeName $typeName
        if($result -ne $null) {
            return $true
        }
    }

    foreach($childItem in $item.ProjectItems) {
        $result = Find-SpecificationInItem -item $childItem -typeName $typeName
        if($result) {
            return $true
        }
    }
}

function Find-Specification($project, $typeName) {

    Write-Host "Searching for $($typeName)"

    foreach ($item in $project.ProjectItems) {
        $result = Find-SpecificationInItem -item $item -typeName $typeName
        if($result) {
            return $true
        }
    }

    $false
}

function Add-ServiceSpecification {
    $found = Find-Specification -project $project -typeName "MassTransit.Hosting.IServiceSpecification"
    if($found -eq $false) {
    	Write-Host "Adding ServiceSpecification.cs"

        $ns = $project.Properties.Item("DefaultNamespace").Value
        $projectPath = [System.IO.Path]::GetDirectoryName($project.FullName)
        $specificationPath = [System.IO.Path]::Combine($projectPath, "ServiceSpecification.cs")

        Get-Content "$installPath\Tools\ServiceSpecificationTemplate.cs" | ForEach-Object { $_ -replace "DefaultNamespace", $ns } | Set-Content ($specificationPath)

        $project.ProjectItems.AddFromFile($specificationPath);
    }
}

function Add-EndpointSpecification {
    $found = Find-Specification -project $project -typeName "MassTransit.Hosting.IEndpointSpecification"
    if($found -eq $false) {
    	Write-Host "Adding EndpointSpecification.cs"

        $ns = $project.Properties.Item("DefaultNamespace").Value
        $projectPath = [System.IO.Path]::GetDirectoryName($project.FullName)
        $specificationPath = [System.IO.Path]::Combine($projectPath, "EndpointSpecification.cs")

        Get-Content "$installPath\Tools\EndpointSpecificationTemplate.cs" | ForEach-Object { $_ -replace "DefaultNamespace", $ns } | Set-Content ($specificationPath)

        $project.ProjectItems.AddFromFile($specificationPath);
    }
}

function Has-ProjectItem($project, $name) {
    Write-Host "Searching for $($name) in project"
    foreach ($item in $project.ProjectItems) {
        if($item.Name -eq $name) {
            return $true
        }
    }

    $false	
}

function Add-LoggingConfig {
    $found = Has-ProjectItem -project $project -name "MassTransit.Host.log4net.config"
    if($found -eq $false) {
    	Write-Host "Adding MassTransit.Host.log4net.config to project and setting copy to output"
        $projectPath = [System.IO.Path]::GetDirectoryName($project.FullName)
        $targetPath = [System.IO.Path]::Combine($projectPath, "MassTransit.Host.log4net.config")
		$sourcePath = "$installPath\Tools\MassTransit.Host.log4net.config"
		
		Copy-Item -Path $sourcePath -Destination $targetPath
        $projectItem = $project.ProjectItems.AddFromFile($targetPath);
		$projectItem.Properties.Item("CopyToOutputDirectory").Value = [int]2
	}
}

function Add-StartProgram {
	[xml] $prjXml = Get-Content $project.FullName
	foreach($PropertyGroup in $prjXml.project.ChildNodes) {
		if($PropertyGroup.StartAction -ne $null) {
			return
		}
	}

    $exeName = "MassTransit.Host.exe"

	$propertyGroupElement = $prjXml.CreateElement("PropertyGroup", $prjXml.Project.GetAttribute("xmlns"));
	$startActionElement = $prjXml.CreateElement("StartAction", $prjXml.Project.GetAttribute("xmlns"));
	$propertyGroupElement.AppendChild($startActionElement) | Out-Null
	$propertyGroupElement.StartAction = "Program"
	$startProgramElement = $prjXml.CreateElement("StartProgram", $prjXml.Project.GetAttribute("xmlns"));
	$propertyGroupElement.AppendChild($startProgramElement) | Out-Null
	$propertyGroupElement.StartProgram = "`$(ProjectDir)`$(OutputPath)$exeName"
	$prjXml.project.AppendChild($propertyGroupElement) | Out-Null
	$writerSettings = new-object System.Xml.XmlWriterSettings
	$writerSettings.OmitXmlDeclaration = $false
	$writerSettings.NewLineOnAttributes = $false
	$writerSettings.Indent = $true
	$projectFilePath = Resolve-Path -Path $project.FullName
	$writer = [System.Xml.XmlWriter]::Create($projectFilePath, $writerSettings)
	$prjXml.WriteTo($writer)
	$writer.Flush()
	$writer.Close()
}

Add-ServiceSpecification
Add-EndpointSpecification
Add-LoggingConfig

$project.Save()

Add-StartProgram