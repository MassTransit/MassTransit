$:.push File.expand_path("../lib", __FILE__)
require 'semver'

Gem::Specification.new do |spec|
  spec.name = "semver2"
  spec.version = SemVer.find.format '%M.%m.%p'
  spec.summary = "Semantic Versioning"
  spec.description = "maintain versions as per http://semver.org"
  spec.email = "henrik@haf.se"
  spec.authors = ["Francesco Lazzarino", "Henrik Feldt"]
  spec.homepage = 'https://github.com/haf/semver'
  spec.executables << 'semver'
  spec.files = [".semver", "semver2.gemspec", "README.md"] + Dir["lib/**/*.rb"] + Dir['bin/*']
  spec.has_rdoc = true
end
