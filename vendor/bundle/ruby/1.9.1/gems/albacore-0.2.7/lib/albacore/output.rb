require 'albacore/albacoretask'
require 'erb'
require 'ostruct'
require 'fileutils'

class OutputBuilder
  include FileUtils
  
  def initialize(dir_to, dir_from)
    @dir_to = dir_to
    @dir_from = dir_from
  end
  
  def dir(dir)
    cp_r "#{@dir_from}/#{dir}", @dir_to
  end
  
  def file(f)
    file(f,f)
  end
    
  def file(f, ft)
    #todo find more elegant way to create base dir if missing for file.
    initialize_to_path(ft)
    cp "#{@dir_from}/#{f}", "#{@dir_to}/#{ft}"
  end
  
  def erb(f, ft, locals)
    initialize_to_path(ft)
    erb = ERB.new(File.read("#{@dir_from}/#{f}"))
    File.open("#{@dir_to}/#{ft}", 'w') {|f| f.write erb.result(ErbBinding.new(locals).get_binding)}
  end
  
  def self.output_to(dir_to, dir_from)
    rmtree dir_to
    mkdir dir_to
    yield OutputBuilder.new(dir_to, dir_from)
  end
  
private
  def initialize_to_path(ft)
    topath = File.dirname("#{@dir_to}/#{ft}")
    mkdir_p topath unless File.exist? topath
    topath
  end
end

class ErbBinding < OpenStruct
  def get_binding
    return binding()
  end
end
  

class Output
  include Albacore::Task

  def initialize
    super()
    
    @files = []
    @erbs = []
    @directories = []
  end
    
  def execute()
    fail_with_message 'No base dir' if @from_dir.nil?
    fail_with_message 'No output dir' if @to_dir.nil?
    
    OutputBuilder.output_to(@to_dir, @from_dir)  do |o|
        @directories.each { |f| o.dir f }
        @files.each { |f| o.file *f }
        @erbs.each { |f| o.erb *f }
    end
  end
  
  def file(f, opts={})
    f_to = opts[:as] || f
    @files << [f,f_to]
  end

  def erb(f, opts={})
    f_to = opts[:as] || f
    @erbs << [f,f_to,opts[:locals]||{}]
  end
  
  def dir(d)
    @directories << d
  end
  
  def from(from_dir)
    @from_dir = from_dir
  end

  def to(to_dir)
    @to_dir = to_dir
  end
  
end
