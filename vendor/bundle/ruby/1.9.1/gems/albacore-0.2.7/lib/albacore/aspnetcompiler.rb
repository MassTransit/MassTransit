require 'albacore/albacoretask'
require 'albacore/config/aspnetcompilerconfig'
require 'albacore/support/supportlinux'

class AspNetCompiler
  include Albacore::Task
  include Albacore::RunCommand
  include Configuration::AspNetCompiler
  include SupportsLinuxEnvironment

  # http://msdn.microsoft.com/en-us/library/ms164291.aspx
  
  attr_accessor  :clean,                              # Optional Boolean parameter. If this parameter is true, the precompiled application will be built clean. Any previously compiled components will be recompiled. The default value is false. This parameter corresponds to the -c switch on aspnet_compiler.exe.
                 #:allow_partially_trusted_callers,    # Optional Boolean parameter. If this parameter is true, the strong-name assembly will allow partially trusted callers.
                 :debug,                              # Optional Boolean parameter. If this parameter is true, debug information (.PDB file) is emitted during compilation. The default value is false. This parameter corresponds to the -d switch on aspnet_compiler.exe.
                 :delay_sign,                         # Optional Boolean parameter. If this parameter is true, the assembly is not fully signed when created.
                 :fixed_names,                        # Optional Boolean parameter. If this parameter is true, the compiled assemblies will be given fixed names.
                 :force,                              # Optional Boolean parameter. If this parameter is true, the task will overwrite the target directory if it already exists. Existing contents are lost. The default value is false. This parameter corresponds to the -f switch on aspnet_compiler.exe.
                 #:key_container,                      # Optional String parameter. Specifies a strong name key container.
                 #:key_file,                           # Optional String parameter. Specifies the physical path to the strong name key file..
                 #:metabase_path,                      # Optional String parameter. Specifies the full IIS metabase path of the application. This parameter cannot be combined with the VirtualPath or PhysicalPath parameters. This parameter corresponds to the -m switch on aspnet_compiler.exe.
                 :physical_path,                      # Optional String parameter. Specifies the physical path of the application to be compiled. If this parameter is missing, the IIS metabase is used to locate the application. This parameter corresponds to the -p switch on aspnet_compiler.exe.
                 #:target_framework_moniker,           # Optional String parameter. Specifies the TargetFrameworkMoniker indicating which .NET Framework version of aspnet_compiler.exe should be used. Only accepts .NET Framework monikers.
                 :target_path,                        # Optional String parameter. Specifies the physical path to which the application is compiled. If not specified, the application is precompiled in-place.
                 :updateable,                         # Optional Boolean parameter. If this parameter is true, the precompiled application will be updateable. The default value is false. This parameter corresponds to the -u switch on aspnet_compiler.exe.
                 :virtual_path                        # Optional String parameter. The virtual path of the application to be compiled. If PhysicalPath specified, the physical path is used to locate the application. Otherwise, the IIS metabase is used, and the application is assumed to be in the default site. This parameter corresponds to the -v switch on aspnet_compiler.exe.

  def initialize
    @clean = false
    @debug = false
    @delay_sign = false
    @fixed_names = false
    @force = false
    @updateable = false
    @virtual_path = '/'
    super()
    update_attributes aspnetcompiler.to_hash
  end

  def execute
    params = []
    params << "-v #{@virtual_path}" unless @virtual_path.nil?
    params << "-p #{format_path(@physical_path)}" unless @physical_path.nil?
    params << "-c" if @clean
    params << "-delaysign" if @delay_sign
    params << "-fixednames" if @fixed_names
    params << "-d" if @debug
    params << "-u" if @updateable
    params << "-f" if @force
    params << format_path(@target_path) unless @target_path.nil?
    
    result = run_command "AspNetCompiler", params
    
    failure_message = 'AspNetCompiler Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end
  
end
