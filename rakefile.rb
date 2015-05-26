COPYRIGHT = "Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al. - All rights reserved."

include FileTest
require 'albacore'
require 'semver'

PRODUCT = 'MassTransit'
CLR_TOOLS_VERSION = 'v4.0.30319'
BUILD_CONFIG = ENV['BUILD_CONFIG'] || "Release"
BUILD_CONFIG_KEY = 'NET45'
BUILD_PLATFORM = ''
TARGET_FRAMEWORK_VERSION = "v4.5"
MSB_USE = :net4
OUTPUT_PATH = 'net-4.5'

props = {
  :src => File.expand_path("src"),
  :lib => File.expand_path("lib"),
  :build_support => File.expand_path("build_support"),
  :stage => File.expand_path("build_output"),
  :output => File.join( File.expand_path("build_output"), OUTPUT_PATH ),
  :artifacts => File.expand_path("build_artifacts"),
  :projects => ["MassTransit", "MassTransit.RuntimeServices"],
  :keyfile => File.expand_path("MassTransit.snk")
}

desc "Default + tests"
task :default => [:clean, :restore, :compile, :nuget, :package]

desc "Update the common version information for the build. You can call this task without building."
assemblyinfo :global_version => [:versioning] do |asm|
  # Assembly file config
  asm.product_name = PRODUCT
  asm.description = "MassTransit is a distributed application framework for .NET http://masstransit-project.com"
  asm.version = FORMAL_VERSION
  asm.file_version = FORMAL_VERSION
  asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}",
    :ComVisibleAttribute => false,
    :CLSCompliantAttribute => true
  asm.copyright = COPYRIGHT
  asm.output_file = 'src/SolutionVersion.cs'
  asm.namespaces "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security"
end

desc "Prepares the working directory for a new build"
task :clean do
  FileUtils.rm_rf props[:artifacts]
  FileUtils.rm_rf props[:stage]
  # work around latency issue where folder still exists for a short while after it is removed
  waitfor { !exists?(props[:stage]) }
  waitfor { !exists?(props[:artifacts]) }

  Dir.mkdir props[:stage]
  Dir.mkdir props[:artifacts]
end

task :compile_samples => [:build_starbucks] do ; end

desc "Compiles MT into build_output"
task :compile => [:versioning, :global_version, :build, :copy_signed, :build_unsigned, :copy_unsigned] do ; end

task :copy_signed do
  puts 'Copying unmerged dependencies to output folder'

  copyOutputFiles File.join(props[:src], "MassTransit/bin/#{BUILD_CONFIG}"), "MassTransit.{dll,pdb,xml}", props[:output]
  copyOutputFiles File.join(props[:src], "MassTransit.AutomatonymousIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.AutomatonymousIntegration.{dll,pdb,xml}", props[:output]

  copyOutputFiles File.join(props[:src], "Persistence/MassTransit.NHibernateIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.NHibernateIntegration.{dll,pdb,xml}", File.join(props[:output], "Persistence/NHibernate")

  copyOutputFiles File.join(props[:src], "MassTransit.QuartzIntegration/bin/Release"), "MassTransit.QuartzIntegration.{dll,pdb,xml}", File.join(props[:output], 'QuartzIntegration')

  copyOutputFiles File.join(props[:src], "MassTransit.QuartzService/bin/Release"), "MassTransit.QuartzService.exe", File.join(props[:output], 'QuartzService')
  copyOutputFiles File.join(props[:src], "MassTransit.QuartzService/bin/Release"), "*.dll", File.join(props[:output], 'QuartzService')
  copyOutputFiles File.join(props[:src], "MassTransit.QuartzService/bin/Release"), "*.config", File.join(props[:output], 'QuartzService')

  outtst = File.join(props[:output], "Testing")
    copyOutputFiles File.join(props[:src], "MassTransit.TestFramework/bin/#{BUILD_CONFIG}"), "MassTransit.TestFramework.{dll,pdb,xml}", outtst

  outl = File.join(props[:output], "Logging")
    copyOutputFiles File.join(props[:src], "Loggers/MassTransit.Log4NetIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.Log4NetIntegration.{dll,pdb,xml}", outl
    copyOutputFiles File.join(props[:src], "Loggers/MassTransit.NLogIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.NLogIntegration.{dll,pdb,xml}", outl

  copyOutputFiles File.join(props[:src], "MassTransit.Reactive/bin/#{BUILD_CONFIG}"), "MassTransit.*.{dll,pdb,xml}", File.join(props[:output], "Reactive")

  outc = File.join(props[:output], "Containers")
    copyOutputFiles File.join(props[:src], "Containers/MassTransit.UnityIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.UnityIntegration.{dll,pdb,xml}", outc
    copyOutputFiles File.join(props[:src], "Containers/MassTransit.WindsorIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.WindsorIntegration.{dll,pdb,xml}", outc
    copyOutputFiles File.join(props[:src], "Containers/MassTransit.NinjectIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.NinjectIntegration.{dll,pdb,xml}", outc
    copyOutputFiles File.join(props[:src], "Containers/MassTransit.AutofacIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.AutofacIntegration.{dll,pdb,xml}", outc

  outt = File.join(props[:output], "Transports")
    copyOutputFiles File.join(props[:src], "MassTransit.AzureServiceBusTransport/bin/#{BUILD_CONFIG}"), "MassTransit.AzureServiceBusTransport.{dll,pdb,xml}", File.join(outt, "AzureServiceBus")
    copyOutputFiles File.join(props[:src], "MassTransit.RabbitMqTransport/bin/#{BUILD_CONFIG}"), "MassTransit.RabbitMqTransport.{dll,pdb,xml}", File.join(outt, "RabbitMQ")
end

desc "Copying Services"
task :copy_unsigned => [:build_unsigned] do
  puts "Copying services"

   outc = File.join(props[:output], "Containers")
    copyOutputFiles File.join(props[:src], "Containers/MassTransit.StructureMapIntegration/bin/#{BUILD_CONFIG}"), "MassTransit.StructureMapIntegration.{dll,pdb,xml}", outc
 
end

task :copy_samples => [:compile_samples] do

  targ = File.join(props[:stage], 'Samples', 'Starbucks')
  src = File.join(props[:src], "Samples", "Starbucks")

    copyOutputFiles props[:output], "MassTransit.dll", targ
    copyOutputFiles src, "Starbucks.Barista/bin/#{BUILD_CONFIG}/MassTransit.Log4NetIntegration.dll", targ
    copyOutputFiles File.join(src, "Starbucks.Customer/bin/#{BUILD_CONFIG}"), "{log4net,Magnum,MassTransit.StructureMapIntegration,MassTransit.Transports.Msmq,StructureMap}.dll", targ
    copyOutputFiles File.join(src, "Starbucks.Barista/bin/#{BUILD_CONFIG}"), "{MassTransit.WindsorIntegration,Castle.Windsor,Castle.Core,Topshelf}.dll", targ
    copyOutputFiles File.join(src, "Starbucks.Cashier/bin/#{BUILD_CONFIG}"), "{MassTransit.NinjectIntegration,Ninject}.dll", targ
    copyOutputFiles File.join(src, "Starbucks.Cashier/bin/#{BUILD_CONFIG}"), "Starbucks.Cashier.exe", targ
    copyOutputFiles File.join(src, "Starbucks.Cashier/bin/#{BUILD_CONFIG}"), "cashier.log4net.xml", targ
    copyOutputFiles File.join(src, "Starbucks.Barista/bin/#{BUILD_CONFIG}"), "Starbucks.Barista.exe", targ
    copyOutputFiles File.join(src, "Starbucks.Barista/bin/#{BUILD_CONFIG}"), "barista.log4net.xml", targ
    copyOutputFiles File.join(src, "Starbucks.Customer/bin/#{BUILD_CONFIG}"), "Starbucks.Customer.exe", targ
    copyOutputFiles File.join(src, "Starbucks.Customer/bin/#{BUILD_CONFIG}"), "customer.log4net.xml", targ
    copyOutputFiles File.join(src, "Starbucks.Customer/bin/#{BUILD_CONFIG}"), "Starbucks.Messages.dll", targ

  targ = File.join(props[:stage], 'Samples', 'Distributor')
  src = File.join(props[:src], "Samples", "Distributor")

    copyOutputFiles props[:output], "MassTransit.dll", targ
    copyOutputFiles src, "Grid.Distributor.Activator/bin/#{BUILD_CONFIG}/MassTransit.Log4NetIntegration.dll", targ

    copyOutputFiles File.join(src, "Grid.Distributor.Activator/bin/#{BUILD_CONFIG}"), "{log4net,Magnum,MassTransit.StructureMapIntegration,MassTransit.Transports.Msmq,StructureMap,Topshelf}.dll", targ
    copyOutputFiles File.join(src, "Grid.Distributor.Activator/bin/#{BUILD_CONFIG}"), "Grid.Distributor.Activator.exe", targ
    copyOutputFiles File.join(src, "Grid.Distributor.Activator/bin/#{BUILD_CONFIG}"), "Grid.Distributor.Shared.dll", targ
    copyOutputFiles File.join(src, "Grid.Distributor.Activator/bin/#{BUILD_CONFIG}"), "*.config", targ
    copyOutputFiles File.join(src, "Grid.Distributor.Worker/bin/#{BUILD_CONFIG}"), "Grid.Distributor.Worker.exe", targ
    copyOutputFiles File.join(src, "Grid.Distributor.Worker/bin/#{BUILD_CONFIG}"), "*.config", targ
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit/MassTransit.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit.Tests/MassTransit.Tests.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Containers/MassTransit.AutofacIntegration/MassTransit.AutofacIntegration.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Containers/MassTransit.Containers.Tests/MassTransit.Containers.Tests.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Containers/MassTransit.NinjectIntegration/MassTransit.NinjectIntegration.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Containers/MassTransit.UnityIntegration/MassTransit.UnityIntegration.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Containers/MassTransit.StructureMapIntegration/MassTransit.StructureMapIntegration.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Loggers/MassTransit.Log4NetIntegration/MassTransit.Log4NetIntegration.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Loggers/MassTransit.NLogIntegration/MassTransit.NLogIntegration.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Persistence/MassTransit.NHibernateIntegration/MassTransit.NHibernateIntegration.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/Persistence/MassTransit.NHibernateIntegration.Tests/MassTransit.NHibernateIntegration.Tests.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit.RabbitMqTransport/MassTransit.RabbitMqTransport.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit.RabbitMqTransport.Tests/MassTransit.RabbitMqTransport.Tests.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit.AzureServiceBusTransport/MassTransit.AzureServiceBusTransport.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit.AzureServiceBusTransport.Tests/MassTransit.AzureServiceBusTransport.Tests.csproj'
end


desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit.Reactive/MassTransit.Reactive.csproj'
end

desc "restores missing packages"
msbuild :restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = 'src/MassTransit.Reactive.Tests/MassTransit.Reactive.Tests.csproj'
end

desc "Only compiles the application."
msbuild :build do |msb|
    msb.properties :Configuration => BUILD_CONFIG,
        :BuildConfigKey => BUILD_CONFIG_KEY,
        :TargetFrameworkVersion => TARGET_FRAMEWORK_VERSION,
        :Platform => 'Any CPU'
    msb.properties[:TargetFrameworkVersion] = TARGET_FRAMEWORK_VERSION unless BUILD_CONFIG_KEY == 'NET35'
    msb.use :net4 #MSB_USE
    msb.targets :Rebuild
    msb.properties[:SignAssembly] = 'true'
    msb.properties[:AssemblyOriginatorKeyFile] = props[:keyfile]
    msb.solution = 'src/MassTransit.sln'
end

desc "Only compiles the application."
msbuild :build_unsigned do |msb|
  msb.properties :Configuration => BUILD_CONFIG + "Unsigned",
      :BuildConfigKey => BUILD_CONFIG_KEY,
      :TargetFrameworkVersion => TARGET_FRAMEWORK_VERSION,
      :Platform => 'Any CPU'
  msb.properties[:TargetFrameworkVersion] = TARGET_FRAMEWORK_VERSION unless BUILD_CONFIG_KEY == 'NET35'
  msb.use :net4 #MSB_USE
  msb.targets :Build
  msb.solution = 'src/MassTransit.sln'
end

msbuild :build_starbucks do |msb|
  msb.properties :Configuration => BUILD_CONFIG,
      :BuildConfigKey => BUILD_CONFIG_KEY,
      :TargetFrameworkVersion => TARGET_FRAMEWORK_VERSION,
      :Platform => 'Any CPU'
  msb.properties[:TargetFrameworkVersion] = TARGET_FRAMEWORK_VERSION unless BUILD_CONFIG_KEY == 'NET35'
  msb.use :net4 #MSB_USE
  msb.targets :Build
  msb.solution = 'src/Samples/Starbucks/Starbucks.sln'
end

def copyOutputFiles(fromDir, filePattern, outDir)
  FileUtils.mkdir_p outDir unless exists?(outDir)
  Dir.glob(File.join(fromDir, filePattern)){|file|
    copy(file, outDir) if File.file?(file)
  }
end

task :tests => [:unit_tests]

desc "Runs unit tests"
nunit :unit_tests do |nunit|
  nunit.command = File.join('src', 'packages','NUnit.Runners.2.6.4', 'tools', 'nunit-console.exe')
  nunit.parameters = "/framework=net-4.5", '/timeout=60000', '/domain=multiple', '/nologo', '/labels', "\"/xml=#{File.join(props[:artifacts], 'nunit-test-results-')}#{OUTPUT_PATH}.xml\""
  nunit.assemblies = FileList["tests/MassTransit.Tests.dll", "tests/MassTransit.Containers.Tests.dll"]
end

desc "Runs transport tests (integration)"
task :transport_tests => [:msmq_tests, :rabbitmq_tests]

task :msmq_tests do
  Dir.mkdir props[:artifacts] unless exists?(props[:artifacts])

  runner = NUnitRunner.new(File.join('lib', 'nunit', 'net-2.0',  "nunit-console#{(BUILD_PLATFORM.empty? ? '' : "-#{BUILD_PLATFORM}")}.exe"),
    'tests',
    TARGET_FRAMEWORK_VERSION,
    ['/nothread', '/nologo', '/labels', "\"/xml=#{File.join(props[:artifacts], 'msmq-test-results.xml')}\""])

  runner.run ['MassTransit.Transports.Msmq.Tests'].map{ |assem| "#{assem}.dll" }
end

task :rabbitmq_tests do
  Dir.mkdir props[:artifacts] unless exists?(props[:artifacts])

  runner = NUnitRunner.new(File.join('lib', 'nunit', 'net-2.0',  "nunit-console#{(BUILD_PLATFORM.empty? ? '' : "-#{BUILD_PLATFORM}")}.exe"),
    'tests',
    TARGET_FRAMEWORK_VERSION,
    ['/nothread', '/nologo', '/labels', "\"/xml=#{File.join(props[:artifacts], 'rabbitmq-test-results.xml')}\""])

  runner.run ['MassTransit.Transports.RabbitMQ.Tests'].map{ |assem| "#{assem}.dll" }
end

task :ci => [:default, :package, :moma]

desc "ZIPs up the build results"
zip :package => [:versioning] do |zip|
  zip.dirs = [props[:stage]]
  zip.output_path = File.join(props[:artifacts], "MassTransit-#{NUGET_VERSION}.zip")
end

desc "Runs the MoMA mono analyzer on the project files. Start the executable manually without --nogui to update the profiles once in a while though, or you'll always get the same report from the analyzer."
task :moma => [:compile] do
  puts "Analyzing project fitness for mono:"
  dlls = project_outputs(props).join(' ')
  sh "lib/MoMA/MoMA.exe --nogui --out #{File.join(props[:artifacts], 'MoMA-report.html')} #{dlls}"
end

  # NUSPEC
  # ===================================================

task :all_nuspecs => [:versioning, :mt_nuspec, :mtl4n_nuspec, :mtnlog_nuspec, :mtsm_nuspec, :mtaf_nuspec, :mtni_nuspec, :mtun_nuspec, :mtcw_nuspec, :mtnhib_nuspec, :mtrmq_nuspec, :mttf_nuspec, :mtrx_nuspec, :mtquartz_nuspec]

  directory 'nuspecs'

  nuspec :mt_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'MassTransit is a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "Newtonsoft.Json", "6.0.8"
    nuspec.dependency "NewId", "2.1.2"
    nuspec.output_file = 'nuspecs/MassTransit.nuspec'

    add_files props[:stage], 'MassTransit.{dll,pdb,xml}', nuspec
    nuspec.file(File.join(props[:src], "MassTransit\\**\\*.cs").gsub("/","\\"), "src")
  end

  nuspec :mt_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.AzureServiceBus'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'Azure Service Bus support for MassTransit (a distributed application framework for .NET, including support for MSMQ and RabbitMQ).'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "Newtonsoft.Json", "6.0.8"
    nuspec.dependency "NewId", "2.1.2"
    nuspec.dependency "WindowsAzure.ServiceBus", "2.6.7"
    nuspec.dependency "Microsoft.WindowsAzure.ConfigurationManager", "3.1.0"
    nuspec.output_file = 'nuspecs/MassTransit.AzureServiceBus.nuspec'

  add_files props[:stage], "#{File.join('Transports', 'AzureServiceBus', 'MassTransit.AzureServiceBusTransport.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "MassTransit.AzureServiceBusTransport\\**\\*.cs").gsub("/","\\"), "src")

  end

  nuspec :mt_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.Automatonymous'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'Automatonymous Support for MassTransit (a distributed application framework for .NET, including support for MSMQ and RabbitMQ).'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "NewId", "2.1.2"
    nuspec.dependency "Automatonymous", "3.0.4-beta"
    nuspec.output_file = 'nuspecs/MassTransit.Automatonymous.nuspec'

  add_files props[:stage], "#{File.join('MassTransit.AutomatonymousIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "MassTransit.AutomatonymousIntegration\\**\\*.cs").gsub("/","\\"), "src")

  end

  nuspec :mtl4n_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.Log4Net'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for Log4Net to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "log4net", "2.0.3"
    nuspec.output_file = 'nuspecs/MassTransit.Log4Net.nuspec'

  add_files props[:stage], File.join('Logging', 'MassTransit.Log4NetIntegration.{dll,pdb,xml}'), nuspec
    nuspec.file(File.join(props[:src], "Loggers\\MassTransit.Log4NetIntegration\\**\\*.cs").gsub("/","\\"), "src")
  end

  nuspec :mtnlog_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.NLog'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Henrik Feldt']
    nuspec.owners = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for NLog to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "NLog", "3.2.1"
    nuspec.output_file = 'nuspecs/MassTransit.NLog.nuspec'

    add_files props[:stage], File.join('Logging', 'MassTransit.NLogIntegration.{dll,pdb,xml}'), nuspec
    nuspec.file(File.join(props[:src], "Loggers\\MassTransit.NLogIntegration\\**\\*.cs").gsub("/","\\"), "src")
 
  end

  nuspec :mtcw_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.CastleWindsor'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for Castle Windsor to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "Castle.Windsor", "3.3.0"
    nuspec.dependency "Castle.Core", "3.3.3"
    nuspec.output_file = 'nuspecs/MassTransit.CastleWindsor.nuspec'

  add_files props[:stage], "#{File.join('Containers', 'MassTransit.WindsorIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "Containers\\MassTransit.WindsorIntegration\\**\\*.cs").gsub("/","\\"), "src")
 
  end

  nuspec :mtrmq_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.RabbitMQ'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for RabbitMQ to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "Newtonsoft.Json", "6.0.8"
    nuspec.dependency "NewId", "2.1.2"
    nuspec.dependency "RabbitMQ.Client", "3.5.3"
    nuspec.output_file = 'nuspecs/MassTransit.RabbitMQ.nuspec'

  add_files props[:stage], "#{File.join('Transports', 'RabbitMQ', 'MassTransit.RabbitMqTransport.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "MassTransit.RabbitMqTransport\\**\\*.cs").gsub("/","\\"), "src")
  end


  nuspec :mtaf_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.Autofac'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for Autofac to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "Autofac", "3.5.2"
    nuspec.output_file = 'nuspecs/MassTransit.Autofac.nuspec'

  add_files props[:stage], "#{File.join('Containers', 'MassTransit.AutofacIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "Containers\\MassTransit.AutofacIntegration\\**\\*.cs").gsub("/","\\"), "src")
  end

  nuspec :mtnhib_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.NHibernate'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'An integration library for NHibernate support in MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "NHibernate", "4.0.3"
    nuspec.dependency "Iesi.Collections", "4.0.1"
    nuspec.output_file = 'nuspecs/MassTransit.NHibernate.nuspec'

  add_files props[:stage], "#{File.join('Persistence', 'NHibernate', 'MassTransit.NHibernateIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "Persistence\\MassTransit.NHibernateIntegration\\**\\*.cs").gsub("/","\\"), "src")
  end


  nuspec :mtni_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.Ninject'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for Ninject to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "Ninject", "3.2.2"
    nuspec.dependency "Ninject.Extensions.NamedScope", "3.2.0"
    nuspec.output_file = 'nuspecs/MassTransit.Ninject.nuspec'

  add_files props[:stage], "#{File.join('Containers', 'MassTransit.NinjectIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "Containers\\MassTransit.NinjectIntegration\\**\\*.cs").gsub("/","\\"), "src")
  end


  nuspec :mtsm_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.StructureMap'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for StructureMap to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "StructureMap", "3.1.5.154"
    nuspec.output_file = 'nuspecs/MassTransit.StructureMap.nuspec'

  add_files props[:stage], "#{File.join('Containers', 'MassTransit.StructureMapIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "Containers\\MassTransit.StructureMapIntegration\\**\\*.cs").gsub("/","\\"), "src")
  end


   nuspec :mtun_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.Unity'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This integration library adds support for Unity to MassTransit, a distributed application framework for .NET, including support for MSMQ and RabbitMQ.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "Unity", "3.5.1404"
    nuspec.output_file = 'nuspecs/MassTransit.Unity.nuspec'

  add_files props[:stage], "#{File.join('Containers', 'MassTransit.UnityIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "Containers\\MassTransit.UnityIntegration\\**\\*.cs").gsub("/","\\"), "src")
  end

  nuspec :mttf_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.TestFramework'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This library contains testing helpers for use with MassTransit.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "NewId", "2.1.2"
    nuspec.dependency "NUnit", "2.6.4"
    nuspec.output_file = 'nuspecs/MassTransit.TestFramework.nuspec'

  add_files props[:stage], "#{File.join('Testing', 'MassTransit.TestFramework.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "MassTransit.TestFramework\\**\\*.cs").gsub("/","\\"), "src")
  end

  nuspec :mtrx_nuspec => ['nuspecs'] do |nuspec|
    nuspec.id = 'MassTransit.Reactive'
    nuspec.version = NUGET_VERSION
    nuspec.authors = ['Chris Patterson', 'Dru Sellers', 'Travis Smith']
    nuspec.description = 'This library contains extension methods for using Reactive Extensions with MassTransit.'
    nuspec.project_url = 'http://masstransit-project.com'
    nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
    nuspec.language = "en-US"
    nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
    nuspec.require_license_acceptance
    nuspec.dependency "MassTransit", NUGET_VERSION
    nuspec.dependency "Rx-Core", "2.2.5"
    nuspec.dependency "Rx-Interfaces", "2.2.5"
    nuspec.dependency "Rx-Linq", "2.2.5"
    nuspec.output_file = 'nuspecs/MassTransit.Reactive.nuspec'

  add_files props[:stage], "#{File.join('Reactive', 'MassTransit.Reactive.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "MassTransit.Reactive\\**\\*.cs").gsub("/","\\"), "src")
  end


nuspec :mtquartz_nuspec do |nuspec|
  nuspec.id = 'MassTransit.QuartzIntegration'
  nuspec.version = NUGET_VERSION
  nuspec.authors = ['Chris Patterson', 'Albert Hives']
  nuspec.summary = 'Quartz integration for MassTransit'
  nuspec.description = 'Adds support for Quartz as a message scheduler to MassTransit (used by the MassTransit.QuartzService project)'
  nuspec.title = 'MassTransit.QuartzIntegration'
  nuspec.project_url = 'http://github.com/MassTransit/MassTransit'
  nuspec.icon_url = 'http://MassTransit-project.com/wp-content/themes/pandora/slide.1.png'
  nuspec.language = "en-US"
  nuspec.license_url = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.require_license_acceptance
  nuspec.dependency "MassTransit", NUGET_VERSION
  nuspec.dependency "NewId", "2.1.2"
  nuspec.dependency "Common.Logging.Core", "3.1.0"
  nuspec.dependency "Common.Logging", "3.1.0"
  nuspec.dependency "Newtonsoft.Json", "6.0.8"
  nuspec.dependency "Quartz", "2.3.2"
  nuspec.output_file = 'nuspecs/MassTransit.QuartzIntegration.nuspec'
  add_files props[:stage], "#{File.join('QuartzIntegration', 'MassTransit.QuartzIntegration.{dll,pdb,xml}')}", nuspec
  nuspec.file(File.join(props[:src], "MassTransit.QuartzIntegration\\**\\*.cs").gsub("/","\\"), "src")
end

  # NUGET
  # ===================================================

  directory 'build_artifacts'

desc "Builds the nuget package"
task :nuget => [:versioning, 'build_artifacts', :all_nuspecs] do
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.Automatonymous.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.Log4Net.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.NLog.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.StructureMap.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.Autofac.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.Ninject.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.Unity.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.CastleWindsor.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.NHibernate.nuspec /Symbols -o build_artifacts"
    sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.AzureServiceBus.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.RabbitMQ.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.TestFramework.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.Reactive.nuspec /Symbols -o build_artifacts"
  sh "src/.nuget/nuget.exe pack -BasePath build_output nuspecs/MassTransit.QuartzIntegration.nuspec /Symbols -o build_artifacts"
end

def project_outputs(props)
  props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.dll" }.
    concat( props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.exe" } ).
    find_all{ |path| exists?(path) }
end

desc "publishes (pushes) the nuget package 'NLog.Targets.RabbitMQ'"
nugetpush :nr_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:nr][:nuget_key] + "." + NUGET_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:local]
  nuget.create_only = false
end

task :verify do
  changed_files = `git diff --cached --name-only`.split("\n") + `git diff --name-only`.split("\n")
  if !(changed_files == [".semver", "Rakefile.rb"] or
    changed_files == ["Rakefile.rb"] or
    changed_files == [".semver"] or
    changed_files.empty?)
    raise "Repository contains uncommitted changes; either commit or stash."
  end
end

task :gittag do
  v = SemVer.find
  if `git tag`.split("\n").include?("#{v.to_s}")
    raise "Version #{v.to_s} has already been released! You cannot release it twice."
  end
  puts 'committing'
  `git commit -am "Released version #{v.to_s}"`
  puts 'tagging'
  `git tag #{v.to_s}`
  puts 'pushing'
  `git push`
  `git push --tags`

  puts "MAINTAINERS: now merge into master and then back into develop!!!"
end

desc "publish nugets! (doesn't build)"
task :publish => [:nr_nuget_push]

desc "MAINTAINERS: builds, git tags and pushes nugets"
task :release => [:verify, :default, :gittag, :publish] do
  puts 'done'
end

def add_files stage, what_dlls, nuspec
  [['net45', 'net-4.5']].each{|fw|
    takeFrom = File.join(stage, fw[1], what_dlls)
    Dir.glob(takeFrom).each do |f|
      nuspec.file(f.gsub("/", "\\"), "lib\\#{fw[0]}")
    end
  }
end

def commit_data
  begin
    commit = `git rev-parse --short HEAD`.chomp()[0,6]
    git_date = `git log -1 --date=iso --pretty=format:%ad`
    commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
  rescue Exception => e
    puts e.inspect
    commit = (ENV['BUILD_VCS_NUMBER'] || "000000")[0,6]
    commit_date = Time.new.strftime("%Y-%m-%d %H%M%S")
  end
  [commit, commit_date]
end

task :versioning do
  ver = SemVer.find
  revision = (ENV['BUILD_NUMBER'] || ver.patch).to_i
  var = SemVer.new(ver.major, ver.minor, revision, ver.special)
  
  # extensible number w/ git hash
  ENV['BUILD_VERSION'] = BUILD_VERSION = ver.format("%M.%m.%p%s") + ".#{commit_data()[0]}"
  
  # nuget (not full semver 2.0.0-rc.1 support) see http://nuget.codeplex.com/workitem/1796
  ENV['NUGET_VERSION'] = NUGET_VERSION = ver.format("%M.%m.%p%s")
  
  # purely M.m.p format
  ENV['FORMAL_VERSION'] = FORMAL_VERSION = "#{ SemVer.new(ver.major, ver.minor, revision).format "%M.%m.%p"}"
  puts "##teamcity[buildNumber '#{BUILD_VERSION}']" # tell teamcity our decision
end

def waitfor(&block)
  checks = 0

  until block.call || checks >10
    sleep 0.5
    checks += 1
  end

  raise 'Waitfor timeout expired. Make sure that you aren\'t running something from the build output folders, or that you have browsed to it through Explorer.' if checks > 10
end