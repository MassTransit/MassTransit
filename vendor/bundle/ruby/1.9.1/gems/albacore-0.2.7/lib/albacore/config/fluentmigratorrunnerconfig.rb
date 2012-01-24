require 'albacore/support/openstruct'

module Configuration
  module FluentMigrator
    include Albacore::Configuration

    def fluentmigrator
      @fluentmigratorconfig ||= OpenStruct.new.extend(OpenStructToHash).extend(FluentMigrator)
      yield(@fluentmigratorconfig) if block_given?
      @fluentmigratorconfig
    end
  end
end


