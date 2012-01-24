require 'albacore/support/openstruct'

module Configuration
  module NUnit
    include Albacore::Configuration

    def nunit
      @nunitconfig ||= OpenStruct.new.extend(OpenStructToHash).extend(NUnit)
      yield(@nunitconfig) if block_given?
      @nunitconfig
    end
  end
end

