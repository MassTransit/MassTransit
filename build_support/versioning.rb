require 'semver'

def commit_data
  begin
    commit = `git rev-parse --short HEAD`.chomp()[0,6]
    git_date = `git log -1 --date=iso --pretty=format:%ad`
    commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
  rescue
    commit = ENV['BUILD_VCS_NUMBER'][0,6] || "000000"
    commit_date = Time.new.strftime("%Y-%m-%d %H%M%S")
  end
  [commit, commit_date]
end

task :versioning do
    fv = version SemVer.find.to_s
    #revision = (!fv[3] || fv[3] == 0) ? (ENV['BUILD_NUMBER'] || Time.now.strftime('%j%H')) : fv[3] #  (day of year 0-265)(hour 00-24)
    revision = ENV['BUILD_NUMBER'].to_i || fv[3]
    ENV['BUILD_VERSION'] = BUILD_VERSION = "#{ SemVer.new(fv[0], fv[1], revision).format "%M.%m.%p" }-#{commit_data()[0]}"
    ENV['FORMAL_VERSION'] = FORMAL_VERSION = "#{ SemVer.new(fv[0], fv[1], revision).format "%M.%m.%p"}.0"
end