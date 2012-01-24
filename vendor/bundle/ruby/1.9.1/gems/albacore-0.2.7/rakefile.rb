$: << './'
require 'lib/albacore'
require 'version_bumper'

task :default => ['albacore:sample']

namespace :specs do
  require 'spec/rake/spectask'

  @spec_opts = '--colour --format specdoc'

  desc "Run all specs for albacore"
  Spec::Rake::SpecTask.new :all do |t|
    t.spec_files = FileList['spec/**/*_spec.rb'].exclude{ |f| 
      f if IS_IRONRUBY && (f.include?("zip")) 
    }
    t.spec_opts << @spec_opts
  end
  
  desc "CSharp compiler (csc.exe) specs" 
  Spec::Rake::SpecTask.new :csc do |t|
    t.spec_files = FileList['spec/csc*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "Assembly info functional specs"
  Spec::Rake::SpecTask.new :assemblyinfo do |t|
    t.spec_files = FileList['spec/assemblyinfo*_spec.rb']
    t.spec_opts << @spec_opts
  end
  
  desc "MSBuild functional specs"
  Spec::Rake::SpecTask.new :msbuild do |t|
    t.spec_files = FileList['spec/msbuild*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "SQLServer SQLCmd functional specs" 
  Spec::Rake::SpecTask.new :sqlcmd do |t|
    t.spec_files = FileList['spec/sqlcmd*_spec.rb']
    t.spec_opts << @spec_opts
  end
  
  desc "Nant functional specs"
  Spec::Rake::SpecTask.new :nant do |t|
    t.spec_files = FileList['spec/nant*_spec.rb']
    t.spec_opts << @spec_opts
  end
  
  desc "NCover Console functional specs"
  Spec::Rake::SpecTask.new :ncoverconsole do |t|
    t.spec_files = FileList['spec/ncoverconsole*_spec.rb']
    t.spec_opts << @spec_opts
  end
  
  desc "NCover Report functional specs"
  Spec::Rake::SpecTask.new :ncoverreport do |t|
    t.spec_files = FileList['spec/ncoverreport*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "Ndepend functional specs"
  Spec::Rake::SpecTask.new :ndepend do |t|
    t.spec_files = FileList['spec/ndepend*_spec.rb']
    t.spec_opts << @spec_opts
  end
  
  desc "Zip functional specs"
  Spec::Rake::SpecTask.new :zip do |t|
    t.spec_files = FileList['spec/zip*_spec.rb']
    t.spec_opts << @spec_opts
    end

  desc "XUnit functional specs"
  Spec::Rake::SpecTask.new :xunit do |t|
    t.spec_files = FileList['spec/xunit*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "NUnit functional specs"
  Spec::Rake::SpecTask.new :nunit do |t|
    t.spec_files = FileList['spec/nunit*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "MSTest functional specs"
  Spec::Rake::SpecTask.new :mstest do |t|
    t.spec_files = FileList['spec/mstest*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "MSpec functional specs"
  Spec::Rake::SpecTask.new :mspec do |t|
    t.spec_files = FileList['spec/mspec*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "Exec functional specs"
  Spec::Rake::SpecTask.new :exec do |t|
    t.spec_files = FileList['spec/exec*_spec.rb']
    t.spec_opts << @spec_opts
  end
  
  desc "Docu functional specs"
  Spec::Rake::SpecTask.new :docu do |t|
    t.spec_files = FileList['spec/docu*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "YAML Config functional specs"
  Spec::Rake::SpecTask.new :yamlconfig do |t|
    t.spec_files = FileList['spec/yaml*_spec.rb']
    t.spec_opts << @spec_opts
  end

  desc "FluenMigrator functional specs"
  Spec::Rake::SpecTask.new :fluentmigrator do |t|
    t.spec_files = FileList['spec/fluentmigrator*_spec.rb']
    t.spec_opts << @spec_opts
  end	
  
  desc "Output functional specs"
  Spec::Rake::SpecTask.new :output do |t|
    t.spec_files = FileList['spec/output*_spec.rb']
    t.spec_opts << @spec_opts
  end
    
  desc "NChurn functional specs"
  Spec::Rake::SpecTask.new :nchurn do |t|
    t.spec_files = FileList['spec/nchurn*_spec.rb']
    t.spec_opts << @spec_opts
  end
end

namespace :albacore do  
  Albacore.configure do |config|
    config.yaml_config_folder = "spec/support/yamlconfig"
    config.log_level = :verbose
  end

  desc "Run a complete Albacore build sample"
  task :sample => ['albacore:assemblyinfo',
                   'albacore:assemblyinfo_modify',
                   'albacore:msbuild',
                   'albacore:ncoverconsole',
                   'albacore:ncoverreport',
                   'albacore:mspec',
                   'albacore:nunit',
                   'albacore:xunit',
                   'albacore:mstest',
                   'albacore:fluentmigrator']
  
  desc "Run a sample MSBuild with YAML autoconfig"
  msbuild :msbuild
  
  desc "Run a sample assembly info generator"
  assemblyinfo do |asm|
    asm.version = "0.1.2.3"
    asm.company_name = "a test company"
    asm.product_name = "a product name goes here"
    asm.title = "my assembly title"
    asm.description = "this is the assembly description"
    asm.copyright = "copyright some year, by some legal entity"
    asm.custom_attributes :SomeAttribute => "some value goes here", :AnotherAttribute => "with some data"
    
    asm.output_file = "spec/support/AssemblyInfo/AssemblyInfo.cs"
  end

  desc "Run a sample assembly info modifier"
  assemblyinfo :assemblyinfo_modify do|asm|
    # modify existing
    asm.version = "0.1.2.3"
    asm.company_name = "a test company"

    # new attribute
    asm.file_version = "4.5.6.7"

    asm.input_file = "spec/support/AssemblyInfo/AssemblyInfoInput.test"
    asm.output_file = "spec/support/AssemblyInfo/AssemblyInfoOutput.cs"
  end
  
  desc "Run a sample NCover Console code coverage"
  ncoverconsole do |ncc|
    @xml_coverage = "spec/support/CodeCoverage/test-coverage.xml"
    File.delete(@xml_coverage) if File.exist?(@xml_coverage)
    
    ncc.log_level = :verbose
    ncc.command = "spec/support/Tools/NCover-v3.3/NCover.Console.exe"
    ncc.output :xml => @xml_coverage
    ncc.working_directory = "spec/support/CodeCoverage/nunit"
    
    nunit = NUnitTestRunner.new("spec/support/Tools/NUnit-v2.5/nunit-console-x86.exe")
    nunit.log_level = :verbose
    nunit.assemblies "assemblies/TestSolution.Tests.dll"
    nunit.options '/noshadow'
    
    ncc.testrunner = nunit
  end  
  
  desc "Run a sample NCover Report to check code coverage"
  ncoverreport :ncoverreport => :ncoverconsole do |ncr|
    @xml_coverage = "spec/support/CodeCoverage/test-coverage.xml"
    
    ncr.command = "spec/support/Tools/NCover-v3.3/NCover.Reporting.exe"
    ncr.coverage_files @xml_coverage
    
    fullcoveragereport = NCover::FullCoverageReport.new
    fullcoveragereport.output_path = "spec/support/CodeCoverage/report/output"
    ncr.reports fullcoveragereport
    
    ncr.required_coverage(
    	NCover::BranchCoverage.new(:minimum => 10),
    	NCover::CyclomaticComplexity.new(:maximum => 1)
    )
  end

  desc "Run ZipDirectory example"
  zip do |zip|
    zip.output_path = File.dirname(__FILE__)
    zip.directories_to_zip = "lib", "spec"
    zip.additional_files "README.markdown"
    zip.output_file = 'albacore_example.zip'
  end
  
  desc "Run UnZip example"
  unzip do |zip|
    zip.unzip_path = File.join File.dirname(__FILE__), 'temp'
    zip.zip_file = 'albacore_example.zip'
  end
   
  desc "MSpec Test Runner Example"
  mspec do |mspec|
    mspec.command = "spec/support/Tools/Machine.Specification-v0.2/Machine.Specifications.ConsoleRunner.exe"
    mspec.assemblies "spec/support/CodeCoverage/mspec/assemblies/TestSolution.MSpecTests.dll"
  end

  desc "NUnit Test Runner Example"
  nunit do |nunit|
    nunit.command = "spec/support/Tools/NUnit-v2.5/nunit-console.exe"
    nunit.assemblies "spec/support/CodeCoverage/nunit/assemblies/TestSolution.Tests.dll"
  end

  desc "MSTest Test Runner Example"
  mstest do |mstest|
    mstest.command = "spec/support/Tools/MSTest-2008/mstest.exe"
    mstest.assemblies "spec/support/CodeCoverage/mstest/TestSolution.MsTestTests.dll"
  end

  desc "XUnit Test Runner Example"
  xunit do |xunit|
    xunit.command = "spec/support/Tools/XUnit-v1.5/xunit.console.exe"
    xunit.assembly = "spec/support/CodeCoverage/xunit/assemblies/TestSolution.XUnitTests.dll"
  end   
  
  desc "Exec Task Example"
  exec do |exec|
    exec.command = 'hostname'
  end   
  
  desc "Mono \ xBuild Example"
  mono do |xbuild|
    xbuild.properties :configuration => :release, :platform => 'Any CPU'
    xbuild.targets :clean, :build
    xbuild.solution = "spec/support/TestSolution/TestSolution.sln"
  end

  desc "FluentMigrator Test Runner Example"
  fluentmigrator do |migrator|
    db_file = "#{ENV['TEMP']}/fluentmigrator.sqlite3"
    File.delete(db_file) if File.exist?(db_file) 
    
    migrator.command = "spec/support/Tools/FluentMigrator-0.9/Migrate.exe"
    migrator.target = "spec/support/FluentMigrator/TestSolution.FluentMigrator.dll"
    migrator.provider = "sqlite"
    migrator.connection = "Data Source=#{db_file};"
  end

end

namespace :jeweler do
  require 'jeweler'  
  Jeweler::Tasks.new do |gs|
    gs.name = "albacore"
    gs.summary = "Dolphin-Safe Rake Tasks For .NET Systems"
    gs.description = "Easily build your .NET solutions with Ruby and Rake, using this suite of Rake tasks."
    gs.email = "albacorebuild@gmail.com"
    gs.homepage = "http://albacorebuild.net"
    gs.authors = ["Derick Bailey", "etc"]
    gs.has_rdoc = false  
    gs.files.exclude(
      "albacore.gemspec", 
      ".gitignore", 
      "spec/",
      "pkg/"
    )
  end
end
