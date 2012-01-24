require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module XBuild
    include Albacore::Configuration

    def xbuild
      @xbuildconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@xbuildconfig) if block_given?
      @xbuildconfig
    end
  end
end

