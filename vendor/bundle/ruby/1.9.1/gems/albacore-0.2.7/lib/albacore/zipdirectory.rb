require 'albacore/albacoretask'
require 'zip/zip'
require 'zip/zipfilesystem'
include Zip

class ZipDirectory
  TaskName = :zip
  include Albacore::Task
  
  attr_accessor :output_path, :output_file
  attr_accessor :flatten_zip
  attr_array :directories_to_zip, :additional_files, :exclusions

  def initialize
    @flatten_zip = true
    @exclusions = []
    super()
    update_attributes Albacore.configuration.zip.to_hash
  end
    
  def execute()
    fail_with_message 'Output File cannot be empty' if @output_file.nil?
    return if @output_file.nil?
        
    clean_directories_names
    remove zip_name

    ZipFile.open(zip_name, 'w')  do |zipfile|
      zip_directory(zipfile)
      zip_additional(zipfile)
    end
  end
  
  def clean_directories_names
    return if @directories_to_zip.nil?
    @directories_to_zip.each { |d| d.sub!(%r[/$],'')}
  end
  
  def remove(filename)
    FileUtils.rm filename, :force=>true
  end
  
  def reject_file(f)
    f == zip_name || is_excluded(f)
  end
  
  def is_excluded(f)
    expanded_exclusions.any? do |e|
      return true if e.respond_to? '~' and f =~ e
      e == f
    end
  end

  def expanded_exclusions
    return @expanded_exclusions unless @expanded_exclusions.nil?

    regexp_exclusions, string_exclusions = @exclusions.partition {|x| x.respond_to? '~' }
    @expanded_exclusions = [].concat(regexp_exclusions)
    
    @directories_to_zip.each do |dir|
      Dir.chdir(dir) do
        string_exclusions.each do |ex|
          exclusions = Dir.glob(ex)
          exclusions = exclusions.map {|x| File.join(dir, x) } unless exclusions[0] == ex
          exclusions << ex if exclusions.empty?
          @expanded_exclusions += exclusions
        end
      end
    end

    @expanded_exclusions
  end
  
  def zip_name()
    @output_path = set_output_path
    File.join(@output_path, @output_file)
  end
  
  def set_output_path()
    path = ''
    path = @directories_to_zip.first unless @directories_to_zip.nil?
    path = @output_path unless @output_path.nil?
    return path
  end
    
  
  def zip_directory(zipfile)
    return if @directories_to_zip.nil?
    @directories_to_zip.each do |d|
      Dir["#{d}/**/**"].reject{|f| reject_file(f)}.each do |file_path|
        file_name = file_path
        file_name = file_path.sub(d + '/','') if @flatten_zip
        zipfile.add(file_name, file_path)
      end
    end
  end
  
  def zip_additional(zipfile)
    return if @additional_files.nil?
    @additional_files.reject{|f| reject_file(f)}.each do |file_path|
      file_name = file_path.split('/').last if @flatten_zip
      zipfile.add(file_name, file_path)
    end
  end
end
