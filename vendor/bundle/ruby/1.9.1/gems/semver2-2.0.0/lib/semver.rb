require 'yaml'
require 'semver/semvermissingerror'

class SemVer

  FILE_NAME = '.semver'
  TAG_FORMAT = 'v%M.%m.%p%s'

  def SemVer.find dir=nil
    v = SemVer.new
    f = SemVer.find_file dir
    v.load f
    v
  end

  def SemVer.find_file dir=nil
    dir ||= Dir.pwd
    raise "#{dir} is not a directory" unless File.directory? dir
    path = File.join dir, FILE_NAME

    Dir.chdir dir do
      while !File.exists? path do
        raise SemVerMissingError, "#{dir} is not semantic versioned", caller if File.dirname(path).match(/(\w:\/|\/)$/i)
        path = File.join File.dirname(path), ".."
        path = File.expand_path File.join(path, FILE_NAME)
        puts "semver: looking at #{path}"
      end
      return path
    end

  end

  attr_accessor :major, :minor, :patch, :special

  def initialize major=0, minor=0, patch=0, special=''
    major.kind_of? Integer or raise "invalid major: #{major}"
    minor.kind_of? Integer or raise "invalid minor: #{minor}"
    patch.kind_of? Integer or raise "invalid patch: #{patch}"

    unless special.empty?
      special =~ /[A-Za-z][0-9A-Za-z-]+/ or raise "invalid special: #{special}"
    end

    @major, @minor, @patch, @special = major, minor, patch, special
  end

  def load file
    @file = file
    hash = YAML.load_file(file) || {}
    @major = hash[:major] or raise "invalid semver file: #{file}"
    @minor = hash[:minor] or raise "invalid semver file: #{file}"
    @patch = hash[:patch] or raise "invalid semver file: #{file}"
    @special = hash[:special]  or raise "invalid semver file: #{file}"
  end

  def save file=nil
    file ||= @file

    hash = {
      :major => @major,
      :minor => @minor,
      :patch => @patch,
      :special => @special
    }

    yaml = YAML.dump hash
    open(file, 'w') { |io| io.write yaml }
  end

  def format fmt
    fmt.gsub! '%M', @major.to_s
    fmt.gsub! '%m', @minor.to_s
    fmt.gsub! '%p', @patch.to_s
    fmt.gsub! '%s', @special.to_s
    fmt
  end

  def to_s
    format TAG_FORMAT
  end

  def <=> other
    maj = major.to_i <=> other.major.to_i
    return maj unless maj == 0

    min = minor.to_i <=> other.minor.to_i
    return min unless min == 0

    pat = patch.to_i <=> other.patch.to_i
    return pat unless pat == 0

    spe = special <=> other.special
    return spec unless spe == 0

    0
  end

  include Comparable
end
