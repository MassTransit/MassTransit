# -*- encoding: utf-8 -*-

Gem::Specification.new do |s|
  s.name = %q{semver2}
  s.version = "2.0.0"

  s.required_rubygems_version = Gem::Requirement.new(">= 0") if s.respond_to? :required_rubygems_version=
  s.authors = [%q{Francesco Lazzarino}, %q{Henrik Feldt}]
  s.date = %q{2011-07-07}
  s.description = %q{maintain versions as per http://semver.org}
  s.email = %q{henrik@haf.se}
  s.executables = [%q{semver}]
  s.files = [%q{bin/semver}]
  s.homepage = %q{https://github.com/haf/semver}
  s.require_paths = [%q{lib}]
  s.rubygems_version = %q{1.8.8}
  s.summary = %q{Semantic Versioning}

  if s.respond_to? :specification_version then
    s.specification_version = 3

    if Gem::Version.new(Gem::VERSION) >= Gem::Version.new('1.2.0') then
    else
    end
  else
  end
end
