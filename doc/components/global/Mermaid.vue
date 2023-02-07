<template>
  <div class="mermaid">
    <slot></slot>
  </div>
</template>

<script lang="ts" setup>

import mermaid from 'mermaid'

mermaid.initialize({
    startOnLoad: true,
    theme: 'dark',
});

onMounted(()=>{
    mermaid.init();
})
</script>
