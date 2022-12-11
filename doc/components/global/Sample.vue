<template>
    <Alert type="info">
        <div style="display:flex; flex-direction: column">
            <div style="display:flex">
                <div>
                    <Icon name="codicon:github-inverted" />
                </div>
                <div id="sample" style="padding-left: 1rem; display: flex; flex-direction: column">
                    <strong>
                        <NuxtLink :to="repoLink" target="_blank" style="margin-right: 0.5rem;">{{title}}</NuxtLink>
                        <Icon name="heroicons:arrow-top-right-on-square" />
                    </strong>
                    <ContentRenderer :value="sample" />
                </div>
            </div>
        </div>
    </Alert>
</template>

<script lang="ts" setup>
type Props = {
    sample: string
}

const props = defineProps<Props>()
const content = await queryContent('samples', props.sample).find();
const sample = content[0]
const repoLink = `https://github.com/${sample.repo}`
let title = sample.repo
if(sample.useTitle) {
    title = sample.title
}
</script>

<style>
#sample div p {
    margin: 0;
    margin-top: 0.5rem;
}
</style>
