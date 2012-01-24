require 'albacore/albacoretask'

class FluentMigratorRunner
	TaskName = :fluentmigrator
	include Albacore::Task
	include Albacore::RunCommand

	attr_accessor :target, :provider, :connection, :namespace, :output, :output_filename, :preview, :steps, :task, :version, :verbose, :script_directory, :profile, :timeout, :show_help

	def initialize(command=nil)
		super()
		update_attributes Albacore.configuration.fluentmigrator.to_hash
		@command = command unless command.nil?
	end

  def get_command_line
    commandline = "#{@command}"
    commandline << get_command_parameters
    @logger.debug "Build FuentMigrator Test Runner Command Line: " + commandline
    commandline
  end

	def get_command_parameters
    if @show_help
      params = " /?"
    else
      params = " /target=\"#{@target}\""
      params << " /provider=#{@provider}"
      params << " /connection=\"#{@connection}\""
      params << " /ns=#{@namespace}" unless @namespace.nil?
      params << " /out" if @output == true
      params << " /outfile=\"#{@output_filename}\"" unless @output_filename.nil?
      params << " /preview" if @preview == true
      params << " /steps=#{@steps}" unless @steps.nil?
      params << " /task=#{@task}" unless @task.nil?
      params << " /version=#{@version}" unless @version.nil?
      params << " /verbose=#{@verbose}" if @verbose == true
      params << " /wd=\"#{@script_directory}\"" unless @script_directory.nil?
      params << " /profile=#{@profile}" unless @profile.nil?
      params << " /timeout=#{@timeout}" unless @timeout.nil?
    end 
    params
	end

	def execute()
    result = run_command "FluentMigrator", get_command_parameters

    failure_message = "FluentMigrator failed. See build log for detail."
    fail_with_message failure_message if !result
	end
end
