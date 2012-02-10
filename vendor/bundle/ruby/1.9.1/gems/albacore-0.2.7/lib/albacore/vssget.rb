require 'albacore/albacoretask'

class VssGet
  TaskName = :vssget
  include Albacore::Task
  include Albacore::RunCommand

  attr_accessor :repository, :project, :path, :username, :password, :recursive, :readonly, :quite
  attr_array :options

  def initialize(command=nil)
    @options=[]
	@quite = true
	@readonly = true
	@recursive = true
    super()
	update_attributes Albacore.configuration.vss.to_hash
    @command = command unless command.nil?
  end

  def get_command_parameters
    command_params = []	
    command_params << "get"
    command_params << @project || "$/"
    command_params << "-Q" if @quite
    command_params << "-R" if @recursive
    command_params << (@readonly ? "-W-" : "-W")
    command_params << "-Y#{@username},#{@password}" unless @username.nil?
    command_params << "\"-GL#{@path}\"" unless @path.nil?
    command_params << @options.join(" ") unless @options.nil?
    command_params
  end

  def execute()
    fail_with_message 'Repository should be provided.' if @repository.nil?

    ENV["SSDIR"] = @repository

    result = run_command "VssGet", get_command_parameters

    failure_message = 'Visual SourceSafe Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end
end
