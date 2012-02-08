# -*- encoding: utf-8 -*-

Gem::Specification.new do |s|
  s.name = "semver2"
  s.version = "2.0.0"

  s.required_rubygems_version = Gem::Requirement.new(">= 0") if s.respond_to? :required_rubygems_version=
  s.authors = ["Francesco Lazzarino", "Henrik Feldt"]
  s.date = "2011-07-07"
  s.description = "maintain versions as per http://semver.org"
  s.email = "henrik@haf.se"
  s.executables = ["semver"]
  s.files = ["bin/semver"]
  s.homepage = "https://github.com/haf/semver"
  s.require_paths = ["lib"]
  s.rubygems_version = "1.8.15"
  s.summary = "Semantic Versioning"

  if s.respond_to? :specification_version then
    s.specification_version = 3

    if Gem::Version.new(Gem::VERSION) >= Gem::Version.new('1.2.0') then
    else
    end
  else
  end
end
