require 'albacore/albacoretask'

class NChurn
  include Albacore::Task
  include Albacore::RunCommand
  
  attr_reader :from, :churn, :top, :report_as, :env_path, :adapter, :exclude, :include, :output

  
  def initialize
    super()
    update_attributes Albacore.configuration.nchurn.to_hash
  end
  

  
  def execute
    command_parameters = []
    command_parameters << ['-d',@from] if @from
    command_parameters << ['-i',@input.to_s] if @input
    command_parameters << ['-c',@churn.to_s] if @churn
    command_parameters << ['-t',@top.to_s] if @top
    command_parameters << ['-r',@report_as.to_s] if @report_as
    command_parameters << ['-p',@env_path.to_s] if @env_path
    command_parameters << ['-a',@adapter.to_s] if @adapter
    command_parameters << ['-x',@exclude] if @exclude
    command_parameters << ['-n',@include] if @include
    command_parameters << ['>',@output] if @output

    result = run_command "NChurn", command_parameters.join(" ")

    failure_msg = 'Churn Analysis Failed. See Build Log For Detail.'
    fail_with_message failure_msg if !result
  end
  
  def churn_precent(p)
    @churn = p/100.0
  end
  def input(p)
    @input = quotes(p)
  end
  def from(p)
    @from = quotes(p.strftime('%d-%m-%Y'))
  end
  def churn(p)
    @churn = p
  end
  def top(p)
    @top= p
  end
  def report_as(p)
    @report_as = p
  end
  def env_path(p)
    @env_path = quotes(p)
  end
  def adapter(p)
    @adapter = p
  end
  def exclude(p)
    @exclude = quotes(p)
  end
  def include(p)
    @include = quotes(p)
  end
  def output(p)
    @output = quotes(p)
  end

  private
  def quotes(s)
   %{"#{s}"}
  end
end
