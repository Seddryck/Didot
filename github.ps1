function Get-GitHub-Headers {
    [CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0)]
        [string] $secretToken
	)
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add('Accept','application/vnd.github+json')
	$headers.Add('X-GitHub-Api-Version','2022-11-28')
	$headers.Add('Authorization',"Bearer $secretToken")
	return $headers
}

function Send-GitHub-Get-Request {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true)]
        [string] $owner,
		[Parameter(Mandatory=$true)]
		[string] $repository,
		[Parameter(Mandatory=$true)]
		[string[]] $segments,
		[Parameter(Mandatory=$true)]
		[System.Collections.IDictionary] $headers
	)
	Invoke-WebRequest `
		-Uri "https://api.github.com/repos/$owner/$repository/$($segments -join '/')" `
		-Headers $headers
}

function Send-GitHub-Post-Request {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true)]
        [object] $body,
		[Parameter(Mandatory=$true)]
		[string] $owner, 
		[Parameter(Mandatory=$true)]
		[string] $repository,
		[Parameter(Mandatory=$true)]
		[string[]] $segments,
		[Parameter(Mandatory=$true)]
		[System.Collections.IDictionary] $headers
	)
	$response = Invoke-WebRequest `
					-Method POST `
					-Uri "https://api.github.com/repos/$owner/$repository/$($segments -join '/')" `
					-Headers $headers `
					-Body $($(ConvertTo-Json $body))
}

function Send-GitHub-FileUpload-Request {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true)]
        [object] $payload,
		[Parameter(Mandatory=$true)]
		[string] $uri,
        [Parameter(Mandatory=$true)]
		[string] $name, 
		[Parameter(Mandatory=$true)]
		[System.Collections.IDictionary] $headers
	)
    if ($headers.Keys -contains "Content-Type") {
        $headers.Remove("Content-Type")
    }
    $headers.Add("Content-Type", "application/zip")

    $uri = $uri -replace '{.*}', ''
    $uri = $uri + "?name=" + $name

	$response = Invoke-WebRequest `
					-Method POST `
					-Uri $uri `
					-Headers $headers `
					-Body $payload
}

function Get-Pull-Request-Title {
    [CmdletBinding()]
	Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0 )]
        [object] $context
	)
	$response = Send-GitHub-Get-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('issues', $context.Id) `
					-Headers $($context.SecretToken | Get-GitHub-Headers)
	return ($response.Content | ConvertFrom-Json).title 
}

function Get-Pull-Request-Labels {
    [CmdletBinding()]
	Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0 )]
        [object] $context
	)
	$response = Send-GitHub-Get-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('issues', $context.Id, 'labels') `
					-Headers $($context.SecretToken | Get-GitHub-Headers)
	return $response.Content | ConvertFrom-Json | Select-Object -ExpandProperty name 
}

function Get-Commit-Associated-Pull-Requests {
    [CmdletBinding()]
	Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0 )]
        [object] $context
	)
	$response = Send-GitHub-Get-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('commits', $context.Id, 'pulls') `
					-Headers $($context.SecretToken | Get-GitHub-Headers)
	[array]$prs = ($response.Content | ConvertFrom-Json).number 
	return $prs
}

function Check-Release-Published {
    [CmdletBinding()]
	Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0 )]
        [object] $context,
		[Parameter(Mandatory=$true)]
		[string] $tag
	)
	$response = Send-GitHub-Get-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('releases') `
					-Headers $($context.SecretToken | Get-GitHub-Headers)
	$existing = ($response.Content | ConvertFrom-Json) `
					| ? {$_.tag_name -eq $tag}`
					| Select-Object -Unique -ExpandProperty 'published_at'
	if ($existing) {
		Write-Host "Release already published at $existing"
		return $true
	}
    return $false
}

function Get-Release-Info {
    [CmdletBinding()]
	Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0 )]
        [object] $context,
		[Parameter(Mandatory=$true)]
		[string] $tag
	)
	$response = Send-GitHub-Get-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('releases') `
					-Headers $($context.SecretToken | Get-GitHub-Headers)
    $json = $response | Select-Object -ExpandProperty Content | ConvertFrom-Json
    return $json | ? {$_.tag_name -eq $tag}
}

function Get-Latest-Release-Info {
    [CmdletBinding()]
	Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0 )]
        [object] $context
	)
	$response = Send-GitHub-Get-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('releases', 'latest') `
					-Headers $($context.SecretToken | Get-GitHub-Headers)
    return $response | Select-Object -ExpandProperty Content | ConvertFrom-Json
}

function Upload-Release-Assets {
    [CmdletBinding()]
	Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0 )]
        [object] $context,
		[Parameter(Mandatory=$true)]
		[string] $tag,
        [Parameter(Mandatory=$true)]
		[string] $path
	)
    $headers = $context.SecretToken | Get-GitHub-Headers
	$info = $context | Get-Release-Info -Tag $tag
    $url = $info | Select-Object -Unique -ExpandProperty 'upload_url'
    
    Get-ChildItem -Path $path -Filter *.zip | ForEach-Object {
        $payload = [System.IO.File]::ReadAllBytes($_.FullName)
        Send-GitHub-FileUpload-Request -Payload $payload -Uri $url -Headers $headers -Name $($_.Name.ToLower())
    }
}

function Post-Pull-Request-Labels {
    [CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0)]
		[object] $context,
		[Parameter(Mandatory=$true)]
        [string[]] $labels
	)
	$body = [PSCustomObject]@{labels=$labels}
	$response = Send-GitHub-Post-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('issues', $context.Id, 'labels') `
					-Headers $($context.SecretToken | Get-GitHub-Headers) `
					-Body $body
}

function Publish-Release {
    [CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0)]
		[object] $context,
		[string] $tag,
		[string] $name,
        [switch] $releaseNotes,
		[string] $discussionCategory
	)
	$body = [PSCustomObject]@{
				tag_name=$tag
				name=$name
				generate_release_notes=$($releaseNotes.IsPresent)
	}
	if ($discussionCategory) {
		$body | Add-Member -MemberType NoteProperty -Name 'discussion_category_name' -Value $discussionCategory
	}
	$response = Send-GitHub-Post-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('releases') `
					-Headers $($context.SecretToken | Get-GitHub-Headers) `
					-Body $body
}

function Download-Release-Asset {
    [CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0)]
		[object] $context,
        [Parameter(Mandatory=$false)]
		[string] $tag,
        [Parameter(Mandatory=$true)]
		[string] $pattern
	)
    if ($null -eq $tag -or $tag -eq "") {
        $tag = $context | Get-Latest-Release-Info | Select-Object -ExpandProperty tag_Name
    }

    $assets = $context | List-Release-Assets -Tag $tag
    $assets = $assets | ? {$_.name -like $pattern}

    if ($null -eq $assets)
    {
        Write-Host "No assets found for pattern $pattern and for tag $tag"
        return
    }

    if ($null -eq $assets.Count) {$count = "1 asset"} else {$count = "$($assets.Count) assets"}
    Write-Host "$count found for pattern $pattern and for tag $tag!"
    $asset = $assets | Select-Object -First 1
    $url = $asset | Select-Object -ExpandProperty browser_download_url 

    Write-Host "Downloading $($asset.name) from $url ..."
	Invoke-WebRequest -Uri $url -OutFile $url.Substring($url.LastIndexOf('/') + 1)
    Write-Host "$($asset.name) downloaded."    
}

function List-Release-Assets {
    [CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true, Position=0)]
		[object] $context,
        [Parameter(Mandatory=$false)]
		[string] $tag
	)
    if ($null -eq $tag -or $tag -eq "") {
        $release = $context | Get-Latest-Release-Info
    } else {
        $release = $context | Get-Release-Info -Tag $tag
    }
    $releaseId = $release | Select-Object -ExpandProperty id

    $response = Send-GitHub-Get-Request `
					-Owner $context.Owner `
					-Repository $context.Repository `
					-Segments @('releases', $releaseId, 'assets') `
					-Headers $($context.SecretToken | Get-GitHub-Headers)
    return $response | Select-Object -ExpandProperty Content | ConvertFrom-Json
}

function Get-Expected-Labels {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true)]
        [string] $title,
		[System.Collections.IDictionary] $mapping
	)
	$labels = @()
	$tokens = $title -Split ':'
	if ($tokens.Length -lt 2) {
		return @()
	}
	
	$conventional = $tokens[0].Trim()
	if ($conventional.IndexOf('(') -gt 0) {
		$conventional = $conventional.SubString(0, $conventional.IndexOf('(') - 1).Trim()
	}

	if ($conventional.EndsWith('!')) {
		if($mapping.ContainsKey('!')) {
			$labels += $mapping['!']
		}
	}

	$conventional = $conventional.TrimEnd('!').Trim()
	if(-not $mapping.ContainsKey($conventional)) {
		return @()
	} else {
		$labels += $mapping[$conventional]
	}
	return $labels
}

function Set-Pull-Request-Expected-Labels {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true, ValueFromPipeline = $true)]
		[object] $context,
		[string] $config
	)

	if ($config) {
		Write-Host "Reading mapping from $config"
		$mapping = (Get-Content $config | ConvertFrom-Json -AsHashtable)
	} else {
		$mapping = @{}
		$mapping.Add('!', 'breaking-change')
		$mapping.Add('build', 'build')
		$mapping.Add('ci', 'build')
		$mapping.Add('chore', 'dependency-update')
		$mapping.Add('docs', 'docs')
		$mapping.Add('feat', 'new-feature')
		$mapping.Add('fix', 'bug')
		$mapping.Add('perf', 'enhancement')
		$mapping.Add('refactor', 'none')
		$mapping.Add('revert', 'none')
		$mapping.Add('style', 'none')
		$mapping.Add('test', 'none')
	}

	$title = $context | Get-Pull-Request-Title
	$existing = $context | Get-Pull-Request-Labels
	$expected = $title | Get-Expected-Labels -Mapping $mapping
	if ($expected.Length -eq 0) {
		throw "Pull Request title is not a valid conventional commit"
	}

	[array]$expected = $expected | ? {$_ -ne 'none'}
	[array]$missing = $expected | ? {-not($existing -contains $_)}
	if ($missing.Length -gt 0) {
		$context | Post-Pull-Request-Labels -Labels $missing
		Write-Host "Pull request #$($context.Id): added following labels: $($missing -Join ',')"
	} else {
		Write-Host "Pull request #$($context.Id): labels already up-to-date."
	}
}
