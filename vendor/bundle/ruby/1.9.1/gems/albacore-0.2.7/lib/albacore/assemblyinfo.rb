require 'albacore/albacoretask'
require 'albacore/assemblyinfolanguages/csharpengine'
require 'albacore/assemblyinfolanguages/vbnetengine'

class AssemblyInfo
  include Albacore::Task
  
  attr_accessor :input_file, :output_file
  attr_accessor :version, :title, :description, :custom_attributes
  attr_accessor :copyright, :com_visible, :com_guid, :company_name, :product_name
  attr_accessor :file_version, :trademark, :lang_engine, :custom_data
  
  attr_array :namespaces
  attr_hash :custom_attributes
  attr_array :custom_data
  
  def initialize
    @namespaces = []
    super()
    update_attributes Albacore.configuration.assemblyinfo.to_hash
  end

  def use(file)
    @input_file = @output_file = file
  end
  
  def execute
    @lang_engine = CSharpEngine.new unless check_lang_engine
    write_assemblyinfo @output_file, @input_file
  end
  
  def write_assemblyinfo(assemblyinfo_file, input_file)
    valid = check_output_file assemblyinfo_file
    return if !valid

    input_data = read_input_file input_file
    asm_data = build_assembly_info_data input_data

    @logger.info "Generating Assembly Info File At: " + File.expand_path(assemblyinfo_file)
    File.open(assemblyinfo_file, 'w') do |f|      
      asm_data.each do |line|
        f.puts line
      end
    end
  end

  def read_input_file(file)
    data = []
    return data if file.nil?

    File.open(file, 'r') do |file|
        file.each_line do |line|
            data << line.strip
        end
    end

    data
  end
  
  def check_output_file(file)
    return true if file
    fail_with_message 'output_file cannot be nil'
    return false
  end
  
  def check_lang_engine
    return !@lang_engine.nil?
  end

  def build_assembly_info_data(data)
    if data.empty?
        data = build_using_statements
    end

    build_attribute(data, "AssemblyTitle", @title) if @title != nil
    build_attribute(data, "AssemblyDescription", @description) if @description != nil
    build_attribute(data, "AssemblyCompany", @company_name) if @company_name != nil
    build_attribute(data, "AssemblyProduct", @product_name) if @product_name != nil
    
    build_attribute(data, "AssemblyCopyright", @copyright) if @copyright != nil
    build_attribute(data, "AssemblyTrademark", @trademark) if @trademark != nil
    
    build_attribute(data, "ComVisible", @com_visible) if @com_visible != nil
    build_attribute(data, "Guid", @com_guid) if @com_guid != nil
    
    build_attribute(data, "AssemblyVersion", @version) if @version != nil
    build_attribute(data, "AssemblyFileVersion", @file_version) if @file_version != nil
    
    data << ""
    if @custom_attributes != nil
      attributes = build_custom_attributes()
      data += attributes
      data << ""
    end

    if @custom_data != nil
      @custom_data.each do |cdata| 
        data << cdata unless data.include? cdata
      end
    end
    
    data
  end

  def build_attribute(data, attr_name, attr_data)
    attr_value = @lang_engine.build_attribute(attr_name, attr_data)
    attr_re = @lang_engine.build_attribute_re(attr_name)
    result = nil
    @logger.debug "Build Assembly Info Attribute: " + attr_value
    data.each do |line|
        break unless result.nil?
        result = line.sub! attr_re, attr_value
    end
    data << attr_value if result.nil?
  end
  
  def build_using_statements
    @namespaces = [] if @namespaces.nil?
    
    @namespaces << "System.Reflection"
    @namespaces << "System.Runtime.InteropServices"
    @namespaces.uniq!
    
    ns = []
    @namespaces.each do |n|
      ns << @lang_engine.build_using_statement(n)
    end
    
    ns
  end  

  def build_custom_attributes()
    attributes = []
    @custom_attributes.each do |key, value|
      attributes << @lang_engine.build_attribute(key, value)
    end
    attributes
  end
  
end
