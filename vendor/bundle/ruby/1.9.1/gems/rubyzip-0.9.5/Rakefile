require 'rubygems'
require 'rake/testtask'

task :default => [:test]

Rake::TestTask.new(:test) do |test|
  test.libs << File.join(File.dirname(__FILE__), 'lib')
  test.libs << File.join(File.dirname(__FILE__), 'test')
  test.pattern = File.join(File.dirname(__FILE__), 'test/alltests.rb')
  test.verbose = true
  Dir.chdir File.join(File.dirname(__FILE__), 'test')
end

