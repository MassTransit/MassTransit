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

        $ns = $project.Properties.Item("DefaultNamespace").Value
        $projectPath = [System.IO.Path]::GetDirectoryName($project.FullName)
        $specificationPath = [System.IO.Path]::Combine($projectPath, "EndpointSpecification.cs")

        Get-Content "$installPath\Tools\EndpointSpecificationTemplate.cs" | ForEach-Object { $_ -replace "DefaultNamespace", $ns } | Set-Content ($specificationPath)

        $project.ProjectItems.AddFromFile($specificationPath);
    }
}


Add-ServiceSpecification
Add-EndpointSpecification
