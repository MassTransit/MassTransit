require 'erb'

class NUnitRunner
	include FileTest

	def initialize(command, working_directory, framework, flags)
		@command = command
		@working_directory = working_directory
		@framework = framework || 'v4.0'
		@flags = flags
		
		# prepare the command
		@cmd = "#{command} /framework=#{@framework}"
		@cmd += ' ' + @flags.join(' ') unless @flags.nil?
	end
	
	def run(assemblies)
		assemblies.each do |assem|
			file = File.expand_path("#{@working_directory}/#{assem}")
			Kernel::system("#{@cmd} \"#{file}\"")
		end
	end
end

class MSBuildRunner
	def self.compile(attributes)
		build_config = attributes.fetch(:build_config)
	    solutionFile = attributes[:solutionfile]
	    
	    attributes[:projFile] = solutionFile
	    attributes[:properties] ||= []
	    attributes[:properties] << "Configuration=#{build_config}"
	    attributes[:extraSwitches] = ["maxcpucount:2", "v:m", "t:rebuild"]
		
		self.runProjFile(attributes);
	end
	
	def self.runProjFile(attributes)
		version = attributes.fetch(:clrversion, 'v4.0.30319')
		build_config = attributes.fetch(:build_config, 'debug')
	    projFile = attributes[:projFile]
		
		frameworkDir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', version)
		msbuildFile = File.join(frameworkDir, 'msbuild.exe')
		
		properties = attributes.fetch(:properties, [])
		
		switchesValue = ""
		
		properties.each do |prop|
			switchesValue += " /property:#{prop}"
		end	
		
		extraSwitches = attributes.fetch(:extraSwitches, [])	
		
		extraSwitches.each do |switch|
			switchesValue += " /#{switch}"
		end	
		
		targets = attributes.fetch(:targets, [])
		targetsValue = ""
		targets.each do |target|
			targetsValue += " /t:#{target}"
		end
		
		Kernel::system("#{msbuildFile} #{projFile} #{targetsValue} #{switchesValue}")
	end
end

class AspNetCompilerRunner
	def self.compile(attributes)
		
		webPhysDir = attributes.fetch(:webPhysDir, '')
		webVirDir = attributes.fetch(:webVirDir, 'This_Value_Is_Not_Used')
		
		frameworkDir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', 'v4.0.30319')
		aspNetCompiler = File.join(frameworkDir, 'aspnet_compiler.exe')

		Kernel::system("#{aspNetCompiler} -nologo -errorstack -c -p #{webPhysDir} -v #{webVirDir}")
	end
end

class AsmInfoBuilder
	attr_reader :buildnumber

	def initialize(baseVersion, properties)
		@properties = properties;
		
		@buildnumber = baseVersion + (ENV["CCNetLabel"].nil? ? '0' : ENV["CCNetLabel"].to_s)
		@properties['Version'] = @properties['InformationalVersion'] = buildnumber;
	end


	
	def write(file)
		template = %q{
using System;
using System.Reflection;
using System.Runtime.InteropServices;

<% @properties.each {|k, v| %>
[assembly: Assembly<%=k%>Attribute("<%=v%>")]
<% } %>
		}.gsub(/^    /, '')
		  
	  erb = ERB.new(template, 0, "%<>")
	  
	  File.open(file, 'w') do |file|
		  file.puts erb.result(binding) 
	  end
	end
end
