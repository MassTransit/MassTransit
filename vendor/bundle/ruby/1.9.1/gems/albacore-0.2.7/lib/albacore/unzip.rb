require 'albacore/albacoretask'
require 'zip/zip'
require 'zip/zipfilesystem'
include Zip

class Unzip
  include Albacore::Task
  
  attr_accessor :destination, :file

  def initialize
    super()
    update_attributes Albacore.configuration.unzip.to_hash
  end
    
  def execute()
    fail_with_message 'Zip File cannot be empty' if @file.nil?
    return if @file.nil?
  
    Zip::ZipFile.open(@file) do |zip_f|
        zip_f.each do |f|
           out_path = File.join(@destination, f.name)
           FileUtils.mkdir_p(File.dirname(out_path))
           zip_f.extract(f, out_path) unless File.exist?(out_path)
        end
      end
  end
end
