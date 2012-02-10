require 'albacore/albacoretask'
require 'albacore/config/cscconfig'
require 'albacore/support/supportlinux'

class CSC
  include Albacore::Task
  include Albacore::RunCommand
  include Configuration::CSC
  include SupportsLinuxEnvironment

  attr_accessor :output, :target, :optimize, :debug, :doc, :main,
    :keyfile, :keycontainer, :delaysign # strong name flags
  attr_array :compile, :references, :resources, :define

  def initialize
    @optimize = false
    super()
    update_attributes csc.to_hash
  end

  def execute
    params = []
    params << @references.map{|r| format_reference(r)} unless @references.nil?
    params << @resources.map{|r| format_resource(r)} unless @resources.nil?
		params << main_entry unless @main.nil?
    params << "\"/out:#{@output}\"" unless @output.nil?
    params << "/target:#{@target}" unless @target.nil?
    params << "/optimize+" if @optimize
    params << "\"/keyfile:#{@keyfile}\"" unless @keyfile.nil?
    params << "\"/keycontainer:#{@keycontainer}\"" unless @keycontainer.nil?
    params << "/delaysign+" if @delaysign
    params << get_debug_param unless @debug.nil?
    params << "/doc:#{@doc}" unless @doc.nil?
    params << get_define_params unless @define.nil?
    params << @compile.map{|f| format_path(f)} unless @compile.nil?

    result = run_command "CSC", params
    
    failure_message = 'CSC Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end

  def get_define_params
    symbols = @define.join(";")
    "/define:#{symbols}"
  end

  def get_debug_param
    case @debug
      when true
        "/debug"
      when :full
        "/debug:full"
      when :pdbonly
        "/debug:pdbonly"
    end
  end

	def main_entry
		"/main:#{@main}"
	end

  def format_resource(resource)
    "/res:#{resource}"
  end
end
