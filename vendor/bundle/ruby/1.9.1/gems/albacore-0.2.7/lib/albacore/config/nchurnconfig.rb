require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NChurn
    include Albacore::Configuration

    def nchurn
      @nchurnconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@nchurnconfig) if block_given?
      @nchurnconfig
    end
  end
end

