require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NAnt
    include Albacore::Configuration

    def nant
      @nantconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@nantconfig) if block_given?
      @nantconfig
    end
  end
end

