# -*- encoding: utf-8 -*-

Gem::Specification.new do |s|
  s.name = "rubyzip"
  s.version = "0.9.5"

  s.required_rubygems_version = Gem::Requirement.new(">= 0") if s.respond_to? :required_rubygems_version=
  s.authors = ["Alan Harper"]
  s.date = "2011-11-26"
  s.email = "alan@aussiegeek.net"
  s.homepage = "http://github.com/aussiegeek/rubyzip"
  s.require_paths = ["lib"]
  s.required_ruby_version = Gem::Requirement.new(">= 1.8.7")
  s.rubygems_version = "1.8.15"
  s.summary = "rubyzip is a ruby module for reading and writing zip files"

  if s.respond_to? :specification_version then
    s.specification_version = 3

    if Gem::Version.new(Gem::VERSION) >= Gem::Version.new('1.2.0') then
    else
    end
  else
  end
end
