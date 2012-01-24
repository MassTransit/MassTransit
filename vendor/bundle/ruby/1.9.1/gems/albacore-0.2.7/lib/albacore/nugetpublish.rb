require 'albacore/albacoretask'
require 'albacore/config/nugetpublishconfig'
require 'albacore/support/supportlinux'

class NuGetPublish
  include Albacore::Task
  include Albacore::RunCommand
  include Configuration::NuGetPublish
  include SupportsLinuxEnvironment
  
  attr_accessor  :id,           # Package Id
                 :version,      # Package Version
                 :apikey,
                 :source,
                 :command

  def initialize(command = "NuGet.exe") # users might have put the NuGet.exe in path
    super()
    update_attributes nugetpublish.to_hash
    @command = command
  end

  def execute
  
    fail_with_message 'id must be specified.' if @id.nil?
    fail_with_message 'version must be specified.' if @version.nil?
    # don't validate @apikey as required, coz it might have been set in the config file using 'SetApiKey'
    
    puts @create_only
    params = []
    params << "publish"
    params << "#{@id}"
    params << "#{@version}"
    params << "#{@apikey}" if @apikey
    params << "-Source #{source}" unless @source.nil?
    
    merged_params = params.join(' ')
    
    @logger.debug "Build NuGet publish Command Line: #{merged_params}"

    result = run_command "NuGet", merged_params
    
    failure_message = 'NuGet Publish Failed. See Build Log For Details'
    fail_with_message failure_message if !result
  end
  
end