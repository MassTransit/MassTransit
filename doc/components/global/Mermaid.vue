<template>
    <slot>
    </slot>
</template>

<script lang="ts" setup>

import mermaid from 'mermaid'

mermaid.setParseErrorHandler(err=>{
    console.log('err', err)
})
mermaid.initialize({
    startOnLoad: false,
    securityLevel: 'loose',
    theme: 'dark',
});

onMounted(()=> {
    mermaid.init({}, '.mermaid')
})
</script>
