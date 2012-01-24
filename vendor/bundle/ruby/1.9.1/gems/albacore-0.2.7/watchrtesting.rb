watch("lib/albacore/.*\.rb") { |md|
  file = File.basename(md[0], ".rb")
  system "spec spec/#{file}_spec.rb"
}

watch("spec/.*_spec\.rb") { |md|
  system "spec #{md[0]}"
}
