require 'semver'

def commit_data
  begin
    commit = `git rev-parse --short HEAD`.chomp()[0,6]
    git_date = `git log -1 --date=iso --pretty=format:%ad`
    commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
  rescue Exception => e
    puts e.inspect
    commit = (ENV['BUILD_VCS_NUMBER'] || "000000")[0,6]
    commit_date = Time.new.strftime("%Y-%m-%d %H%M%S")
  end
  [commit, commit_date]
end

task :versioning do
  ver = SemVer.find
  revision = (ENV['BUILD_NUMBER'] || ver.patch).to_i
  var = SemVer.new(ver.major, ver.minor, revision, ver.special)
  
  # extensible number w/ git hash
  ENV['BUILD_VERSION'] = BUILD_VERSION = ver.format("%M.%m.%p%s") + ".#{commit_data()[0]}"
  
  # nuget (not full semver 2.0.0-rc.1 support) see http://nuget.codeplex.com/workitem/1796
  ENV['NUGET_VERSION'] = NUGET_VERSION = ver.format("%M.%m.%p%s")
  
  # purely M.m.p format
  ENV['FORMAL_VERSION'] = FORMAL_VERSION = "#{ SemVer.new(ver.major, ver.minor, revision).format "%M.%m.%p"}"
  puts "##teamcity[buildNumber '#{BUILD_VERSION}']" # tell teamcity our decision
end