class CSharpEngine
def build_attribute_re(attr_name)
    /^\[assembly: #{attr_name}(.+)/  
  end

  def build_attribute(attr_name, attr_data)
    attribute = "[assembly: #{attr_name}("
    attribute << "#{attr_data.inspect}" if attr_data != nil
    attribute << ")]"
    
    attribute
  end

  def build_using_statement(namespace)
    "using #{namespace};"
  end
    
end
