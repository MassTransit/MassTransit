albacore_root = File.expand_path(File.dirname(__FILE__))
$: << albacore_root
$: << File.join(albacore_root, "albacore")
$: << File.join(albacore_root, "albacore", 'support')
$: << File.join(albacore_root, "albacore", 'config')

IS_IRONRUBY = (defined?(RUBY_ENGINE) && RUBY_ENGINE == "ironruby")

Dir.glob(File.join(albacore_root, 'albacore/*.rb')).each {|f| require f }
