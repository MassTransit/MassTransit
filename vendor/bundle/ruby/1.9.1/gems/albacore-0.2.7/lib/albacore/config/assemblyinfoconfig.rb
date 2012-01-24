require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module AssemblyInfo
    include Albacore::Configuration
    def assemblyinfo
      @asmconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@asmconfig) if block_given?
      @asmconfig
    end
  end
end
