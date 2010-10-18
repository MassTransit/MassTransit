version = File.read(File.expand_path("../VERSION",__FILE__)).strip

Gem::Specification.new do |spec|
  spec.platform    = Gem::Platform::RUBY
  spec.name        = '__RENAME__'
  spec.version     = version
  spec.files = Dir['lib/**/*'] + Dir['docs/**/*'] #+ Dir['bin/**/*']
#  spec.bindir = 'bin'
#  spec.executables << '__REPLACE__'

#  spec.add_dependency('log4net','= 1.2.10')

  spec.summary     = '__REPLACE__'
  spec.description = <<-EOF 
__REPLACE__

EOF
  
  spec.author            = '__REPLACE__'
#  spec.authors            = ['__REPLACE__','__REPLACE__']
  spec.email             = '__REPLACE__'
  spec.homepage          = '__REPLACE__'
  spec.rubyforge_project = '__RENAME__'
end