module SupportsLinuxEnvironment
  attr_accessor :is_linux
  
  def initialize
    @is_linux = RUBY_PLATFORM.include? 'linux'
    super()
  end
  
  def format_reference(reference)
    "\"/reference:#{to_OS_format(reference)}\""
  end

  def format_path(path)
    "\"#{to_OS_format(path)}\""
  end

  def to_OS_format(input)
    formatted_input = @is_linux ? input : input.gsub("/", "\\")
	formatted_input
  end
end


