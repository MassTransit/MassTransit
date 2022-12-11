<template>
  <div class="mermaid">
    <slot></slot>
  </div>
</template>

<script>
export default {
  mounted() {
    import("mermaid/dist/mermaid").then(m => {
      m.initialize({
        startOnLoad: true,
          theme: 'dark'
      });
      m.init();
    });
  }
};
</script>
