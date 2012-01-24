# -*- encoding: utf-8 -*-

Gem::Specification.new do |s|
  s.name = %q{albacore}
  s.version = "0.2.7"

  s.required_rubygems_version = Gem::Requirement.new(">= 0") if s.respond_to? :required_rubygems_version=
  s.authors = [%q{Derick Bailey}, %q{etc}]
  s.date = %q{2011-07-10}
  s.description = %q{Easily build your .NET solutions with Ruby and Rake, using this suite of Rake tasks.}
  s.email = %q{albacorebuild@gmail.com}
  s.extra_rdoc_files = [%q{README.markdown}]
  s.files = [%q{README.markdown}]
  s.homepage = %q{http://albacorebuild.net}
  s.require_paths = [%q{lib}]
  s.rubygems_version = %q{1.8.8}
  s.summary = %q{Dolphin-Safe Rake Tasks For .NET Systems}

  if s.respond_to? :specification_version then
    s.specification_version = 3

    if Gem::Version.new(Gem::VERSION) >= Gem::Version.new('1.2.0') then
      s.add_runtime_dependency(%q<rubyzip>, ["~> 0.9"])
      s.add_development_dependency(%q<nokogiri>, ["~> 1.4"])
      s.add_development_dependency(%q<version_bumper>, ["~> 0.3"])
      s.add_development_dependency(%q<jeweler>, ["~> 1.6"])
      s.add_development_dependency(%q<rspec>, ["~> 1.2"])
      s.add_development_dependency(%q<jekyll>, ["~> 0.8"])
      s.add_development_dependency(%q<watchr>, ["~> 0.7"])
    else
      s.add_dependency(%q<rubyzip>, ["~> 0.9"])
      s.add_dependency(%q<nokogiri>, ["~> 1.4"])
      s.add_dependency(%q<version_bumper>, ["~> 0.3"])
      s.add_dependency(%q<jeweler>, ["~> 1.6"])
      s.add_dependency(%q<rspec>, ["~> 1.2"])
      s.add_dependency(%q<jekyll>, ["~> 0.8"])
      s.add_dependency(%q<watchr>, ["~> 0.7"])
    end
  else
    s.add_dependency(%q<rubyzip>, ["~> 0.9"])
    s.add_dependency(%q<nokogiri>, ["~> 1.4"])
    s.add_dependency(%q<version_bumper>, ["~> 0.3"])
    s.add_dependency(%q<jeweler>, ["~> 1.6"])
    s.add_dependency(%q<rspec>, ["~> 1.2"])
    s.add_dependency(%q<jekyll>, ["~> 0.8"])
    s.add_dependency(%q<watchr>, ["~> 0.7"])
  end
end
