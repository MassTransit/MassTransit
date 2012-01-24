require 'albacore/albacoretask'
require 'albacore/config/nugetpushconfig'
require 'albacore/support/supportlinux'

class NuGetPush
  include Albacore::Task
  include Albacore::RunCommand
  include Configuration::NuGetPush
  include SupportsLinuxEnvironment
  
  attr_accessor  :package,
                 :apikey,
                 :create_only,
                 :source,
                 :command

  def initialize(command = "NuGet.exe") # users might have put the NuGet.exe in path
    super()
    @create_only = false
    update_attributes nugetpush.to_hash
    @command = command
  end

  def execute
  
    fail_with_message 'package must be specified.' if @package.nil?
    # don't validate @apikey as required, coz it might have been set in the config file using 'SetApiKey'
    
    params = []
    params << "push"
    params << "\"#{@package}\""
    params << "#{@apikey}" if @apikey
    params << "-CreateOnly" if @create_only
    params << "-Source #{source}" unless @source.nil?
    
    merged_params = params.join(' ')
    
    @logger.debug "Build NuGet push Command Line: #{merged_params}"

    result = run_command "NuGet", merged_params
    
    failure_message = 'NuGet push Failed. See Build Log For Details'
    fail_with_message failure_message if !result
  end
  
end