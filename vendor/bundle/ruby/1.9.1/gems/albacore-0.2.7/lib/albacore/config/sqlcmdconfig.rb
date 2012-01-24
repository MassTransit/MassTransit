require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module SQLCmd
    include Albacore::Configuration

    def sqlcmd
      @sqlcmdconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@sqlcmdconfig) if block_given?
      @sqlcmdconfig
    end
  end
end

