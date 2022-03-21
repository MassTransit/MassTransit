<template>
    <div class="launch-pad">
      <router-link :to="{ path: this.transport.link }">
      
        <h1>{{ this.transport.text }}</h1>
        <p>{{ this.transport.description }}</p>
        <div style="text-align:right"><small>Hop On!</small></div>
      </router-link>
    </div>
</template>

<script>
export default {
    name: 'launch-pad',
    props: {
        transport: { required: true, type: Object }
    }
}
</script>

<style lang="stylus">
.hero
  .launch-pad
    border-radius: 0.50rem
    
    padding: 1rem
    
    a
      h1
        font-size 1rem
        margin-bottom: 0.25rem
        margin-top: 0.25rem
        border-bottom: 1px solid #eaecef
      p
        margin-top: 0.25rem
        color: #6a8bad
    &:hover
      background: #f0f9ff
</style>