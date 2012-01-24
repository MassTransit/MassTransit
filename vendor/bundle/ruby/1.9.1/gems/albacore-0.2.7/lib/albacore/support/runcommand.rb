require 'albacore/support/attrmethods'

module Albacore
  module RunCommand
    extend AttrMethods
    
    attr_accessor :command, :working_directory
    attr_array :parameters
    
    def initialize
      @working_directory = Dir.pwd
      @parameters = []
      super()
    end
    
    def run_command(name="Command Line", parameters=nil)
      begin
        params = Array.new
        params << parameters unless parameters.nil?
        params << @parameters unless (@parameters.nil? || @parameters.length==0)
        
        cmd = get_command(params)
        @logger.debug "Executing #{name}: #{cmd}"
        
        Dir.chdir(@working_directory) do
          return system(cmd)
        end

      rescue Exception => e
        puts "Error While Running Command Line Tool: #{e}"
        raise
      end
    end

    def get_command(params)
      executable = @command
      unless command.nil?
        executable = File.expand_path(@command) if File.exists?(@command)
      end
      cmd = "\"#{executable}\""
      cmd +=" #{params.join(' ')}" if params.length > 0
      cmd
    end
  end
end
