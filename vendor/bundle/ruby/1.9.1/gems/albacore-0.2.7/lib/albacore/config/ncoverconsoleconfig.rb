require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NCoverConsole
    include Albacore::Configuration

    def ncoverconsole
      @ncoverconsoleconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@ncoverconsoleconfig) if block_given?
      @ncoverconsoleconfig
    end
  end
end

