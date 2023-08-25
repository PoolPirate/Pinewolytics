<script lang="ts">
    import plus from "$lib/static/logo/plus.svg"
    import x from "$lib/static/logo/x.svg"
	import { onMount } from "svelte";

interface Attribute {
    key: string;
    name: string;
}

    var chain: string = "osmosis";
    var messageType: string = "";
    var attributes: Attribute[] = [];

    var tempAttributeKey: string = "";
    var tempAttributeName: string = "";

    var sql: string | null = null;
    var generated: boolean = false;

    function addAttribute() {
        attributes.push({
            key: tempAttributeKey,
            name: tempAttributeName
        });

        attributes = attributes;

        tempAttributeKey = "";
        tempAttributeName = "";
    }

    function removeAttribute(attribute: Attribute) {
        attributes = attributes.filter(x => x != attribute)
    }

    function attributeToTable(attribute: Attribute) {
        var replaced = "a_" + attribute.key.replaceAll("-", "_");
        
        while(replaced.startsWith("_")) {
            replaced.slice(1);
        }

        return replaced;
    }

    function generate() {
        generated = true;

        const tableNames = attributes.map(x => `${attributeToTable(x)}`)

        const tables = tableNames.map(x => `${chain}.core.fact_msg_attributes AS ${x}`)
        const cols = attributes.map(x => `${attributeToTable(x)}.attribute_value AS ${x.name}`)

        sql = `
SELECT ${tableNames[0]}.tx_id, ${tableNames[0]}.block_timestamp, 
${cols.reduce((prev, curr, i, arr) => `${prev}${i != 0 ? "\n" : ""}  ${curr}${i != arr.length - 1 ? "," : ""}`, "")}
FROM 
${tables.reduce((prev, curr, i, arr) => `${prev}${i != 0 ? "\n" : ""}  ${curr}${i != arr.length - 1 ? "," : ""}`, "")}
WHERE ${tableNames[0]}.msg_type = '${messageType}'
${attributes.reduce((prev, curr, i, arr) => `${prev}${lineCheck(i, arr)}`, "")}
        `.trim();
    }


    function lineCheck(index: number, attributes: Attribute[]) {
        const keyCheck = `  AND ${attributeToTable(attributes[index])}.attribute_key = '${attributes[index].key}'`;

        if (index == 0) {
            return keyCheck + (index == attributes.length - 1 ? "" : "\n")
        }

        return keyCheck + ` AND ${attributeToTable(attributes[0])}.tx_id = ${attributeToTable(attributes[index])}.tx_id` +
            ` AND ${attributeToTable(attributes[0])}.msg_index = ${attributeToTable(attributes[index])}.msg_index` +
            ` AND (${attributeToTable(attributes[0])}.msg_group = ${attributeToTable(attributes[index])}.msg_group OR ${attributeToTable(attributes[0])}.msg_group IS NULL AND ${attributeToTable(attributes[index])}.msg_group IS NULL)` +
            (index == attributes.length - 1 ? "" : "\n")
    }

    async function copyGenerated() {
        if (sql == null) {
            return;
        }

        await navigator.clipboard.writeText(sql)
    }
</script>

<div class="w-full h-full flex gap-7 justify-center items-center overflow-auto">
    <div class="w-1/2 flex flex-col gap-2 rounded-lg p-3 transparent-background">
        <h1 class="text-2xl font-bold">Event Decoder</h1>

        <div class="transparent-background rounded-md p-3 flex flex-col gap-1 border-2 border-gray-500">
            <h3 class="font-bold text-lg">Chain</h3>

            <select class="p-1 rounded-md" bind:value={chain}>
                <option value="osmosis">Osmosis</option>
                <option value="cosmos">Cosmos Hub</option>
                <option value="evmos">Evmos</option>
                <option value="axelar">Axelar</option>
                <option value="sei">Sei</option>
            </select>
    
            <h3 class="font-bold text-lg">Message Type</h3>
    
            <input class="p-1 rounded-md" bind:value={messageType} />
    
            <h3 class="font-bold text-lg">Attribute Keys</h3>
    
            <div class="grid grid-cols-[1fr_1fr_35px] gap-2">
                <p>Attribute Key</p>
                <p>SQL Column Name</p>
                <p></p>
                
                {#each attributes as attribute}
                    <p>
                        {attribute.key}
                    </p>
                    <p>
                        {attribute.name}
                    </p>
                    <button on:click={() => removeAttribute(attribute)}>
                        <img src={x} alt="plus icon">
                    </button>
                {/each}

                <input class="p-1 rounded-md" bind:value={tempAttributeKey} />
                <input class="p-1 rounded-md" bind:value={tempAttributeName} />

                <button on:click={addAttribute}>
                    <img  src={plus} alt="plus icon">
                </button>
            </div>
            
        </div>
    
        <button on:click={generate} class="bg-blue-500 p-2 rounded-md text-lg font-bold">Generate</button>
    </div>

    {#if generated}
        <div class="w-1/3 transparent-background p-3 flex flex-col gap-2 rounded-xl">
            <button class="font-bold bg-blue-400 p-1 rounded-md" on:click={copyGenerated}>Copy SQL</button>
            <p class:hidden={!generated} class="whitespace-pre bg-gray-400 font-mono overflow-auto p-2 rounded-md">
                {sql}
            </p>
        </div>
    {/if}



</div>