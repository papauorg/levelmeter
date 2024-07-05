---
layout: page
title: Available scales
permalink: /scales/
---

This is a list of currently pre-generated or manually created scales available from the [github repo]({{ site.data.githubinfo.repositoryUrl }}).

<table id="scales" class="display" style="width: 100%">
    <thead>
        <tr>
            <th>Container Form</th>
            <th>Inner Ø</th>
            <th>Container Height</th>
            <th>Vol.</th>
            <th>Scale Unit</th>
            <th>Description</th>
            <th>Confirmed success</th>
            <th>Needs Transfertape</th>
            <th>Graduation mark types</th>
            <th>Preview</th>
            <th>Definition file</th>
        </tr>
    </thead>
    <tbody>
        {% assign definitions = site.data.scales.definitions %}
        {% for scale_hash in definitions %}
        {% assign scale = scale_hash[1].scale-config %}
        {% if scale %}
        <tr>
            <td>{{ scale.containerForm }}</td>
            <td>{{ scale.diameter }} {{ scale.lengthUnit }}</td>
            <td>{{ scale.height }} {{  scale.lengthUnit }}</td>
            <td>{{ scale.maxVolume }} {{ scale.volumeUnit }}</td>
            <td>{{ scale.volumeUnit }}</td>
            <td>{{ scale.description }}</td>
            <td>{{ scale.successfullyAppliedTo }}</td>
            <td>{{ scale.requiresTransferTape | default: "N/A"}}</td>
            <td>{{ scale.graduationMarkSettings | size }}</td>
            <td>
                <a href="{{ site.data.githubinfo.repositoryUrl }}/blob/main/scales/svgs/{{ scale_hash[0] }}.svg">
                    <img src="https://raw.githubusercontent.com/{{ site.data.githubinfo.repositoryId }}/main/scales/svgs/{{ scale_hash[0] }}.svg" alt="scale preview picture">
                </a>
            </td>
            <td><a href="{{ site.data.githubinfo.repositoryUrl }}/blob/main/scales/definitions/{{ scale_hash[0] }}.json">{{ scale_hash[0] }}.json</a></td>
        </tr>
        {% endif %}
        {% endfor %}
    </tbody>

</table>


<script type="text/javascript">
    $(document).ready( function () {
        $('#scales').DataTable({
            pageLength: 25,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            order: [
                [0, 'asc'], /* container form */
                [4, 'desc'], /* volume unit */
                [3, 'asc'], /* max volume */
                [1, 'asc'], /* diameter */
            ]
        });
    });
</script>
